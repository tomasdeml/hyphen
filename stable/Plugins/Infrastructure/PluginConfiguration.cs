/***********************************************************************\
 * Virtuoso.Miranda.Plugins (Hyphen)                                   *
 * Provides a managed wrapper for API of IM client Miranda.            *
 * Copyright (C) 2006-2009 virtuoso                                    *
 *                    deml.tomas@seznam.cz                             *
 *                                                                     *
 * This library is free software; you can redistribute it and/or       *
 * modify it under the terms of the GNU Lesser General Public          *
 * License as published by the Free Software Foundation; either        *
 * version 2.1 of the License, or (at your option) any later version.  *
 *                                                                     *
 * This library is distributed in the hope that it will be useful,     *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of      *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU   *
 * Lesser General Public License for more details.                     *
\***********************************************************************/

using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Formatters.Binary;
using Virtuoso.Miranda.Plugins.Collections;
using Virtuoso.Miranda.Plugins.Configuration;
using Virtuoso.Miranda.Plugins.Helpers;
using Virtuoso.Miranda.Plugins.Resources;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    [Serializable]
    public abstract class PluginConfiguration
    {
        #region Fields

        private static readonly object SyncObject = new object();

        private static readonly TypeInstanceCache<IStorage> Stores = new TypeInstanceCache<IStorage>();
        private static readonly TypeInstanceCache<IEncryption> Encryptions = new TypeInstanceCache<IEncryption>();

        internal readonly ConfigurationValues values;

        private bool isDirty;

        #endregion

        #region .ctors

        protected PluginConfiguration()
        {
            values = new ConfigurationValues();
        }

        protected virtual void InitializeDefaultConfiguration() { }

        #endregion

        #region Events

        [field: NonSerialized]
        public event EventHandler ConfigurationChanged;

        protected void RaiseChangedEvent()
        {
            if (ConfigurationChanged != null)
                ConfigurationChanged(this, EventArgs.Empty);
        }

        #endregion

        #region Properties

        public ConfigurationValues Values
        {
            get
            {
                return values;
            }
        }

        public bool IsDirty
        {
            get
            {
                return isDirty;
            }
            protected internal set
            {
                isDirty = value;
            }
        }

        #endregion

        #region Methods

        protected virtual void OnBeforeSerialization() { }

        protected virtual void OnAfterDeserialization() { }

        protected void MarkDirty()
        {
            IsDirty = true;
            RaiseChangedEvent();
        }        

        #region Helpers

        private static ConfigurationOptionsAttribute GetOptions(Type configType)
        {
            if (configType == null)
                throw new ArgumentNullException("configType");

            ConfigurationOptionsAttribute options = null;
            Type configAttribType = typeof(ConfigurationOptionsAttribute);

            if (configType.IsDefined(configAttribType, false))
                options = (ConfigurationOptionsAttribute)configType.GetCustomAttributes(configAttribType, false)[0];
            else
                options = new ConfigurationOptionsAttribute();

            return options.Finalize();
        }

        private static byte[] FetchStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (!stream.CanRead)
                throw new ArgumentException();

            byte[] buffer = new byte[stream.Length];

            if (stream.Read(buffer, 0, buffer.Length) != stream.Length)
                throw new IOException();

            return buffer;
        }

        internal static void FlushCaches()
        {
            lock (SyncObject)
            {
                foreach (IStorage storage in Stores.Values)
                    storage.Dispose();

                Stores.Clear();
                Encryptions.Clear();
            }
        }

        #endregion

        #region Save

        public void Save()
        {
            try
            {
                lock (SyncObject)
                {
                    OnBeforeSerialization();

                    ConfigurationOptionsAttribute options = GetOptions(GetType());
                    IStorage storage = Stores.Instantiate(options.Storage);

                    if (options.Encrypt)
                        SerializeEncrypted(storage, options);
                    else
                        Serialize(storage, options);
                }
            }
            catch (IsolatedStorageException isE)
            {
                throw new ConfigurationException(TextResources.ExceptionMsg_UnableToSaveConfiguration_StorageError, isE);
            }
        }

        private void Serialize(IStorage storage, ConfigurationOptionsAttribute options)
        {
            BinaryFormatter serializer = new BinaryFormatter();

            using (Stream stream = storage.OpenWrite(GetType(), options))
                serializer.Serialize(stream, this);
        }

        private void SerializeEncrypted(IStorage storage, ConfigurationOptionsAttribute options)
        {
            IEncryption encryption = Encryptions.Instantiate(options.Encryption);

            using (Stream serializationStream = new MemoryStream(2048))
            {
                new BinaryFormatter().Serialize(serializationStream, this);
                serializationStream.Seek(0, SeekOrigin.Begin);

                byte[] data = FetchStream(serializationStream);
                byte[] protectedData = encryption.Encrypt(data);

                using (Stream stream = storage.OpenWrite(GetType(), options))
                    stream.Write(protectedData, 0, protectedData.Length);
            }
        }

        #endregion

        #region Load

        public static T Load<T>() where T : PluginConfiguration
        {
            try
            {
                lock (SyncObject)
                {
                    Type configType = typeof(T);
                    ConfigurationOptionsAttribute options = GetOptions(configType);

                    IStorage storage = Stores.Instantiate(options.Storage);

                    if (!storage.Exists(configType, options))
                        return GetDefaultConfiguration<T>();

                    using (Stream stream = storage.OpenRead(configType, options))
                    {
                        T result = null;

                        if (options.Encrypt)
                            result = DeserializeEncrypted<T>(stream, options);
                        else
                            result = Deserialize<T>(stream);

                        result.OnAfterDeserialization();
                        return result;
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                throw new ConfigurationException(TextResources.ExceptionMsg_UnableToLoadConfiguration_StorageError, e);                
            }
            catch (Exception e)
            {
                T defaults = GetDefaultConfiguration<T>();
                defaults.Save();

                throw new ConfigurationException(TextResources.ExceptionMsg_UnableToLoadConfiguration_StorageError, e);
            }
        }

        private static T Deserialize<T>(Stream stream) where T : PluginConfiguration
        {
            return new BinaryFormatter().Deserialize(stream) as T;
        }

        private static T DeserializeEncrypted<T>(Stream stream, ConfigurationOptionsAttribute options) where T : PluginConfiguration
        {
            byte[] protectedData = FetchStream(stream);
            byte[] data = Encryptions.Instantiate(options.Encryption).Decrypt(protectedData);

            using (Stream serializedStream = new MemoryStream(data))
                return new BinaryFormatter().Deserialize(serializedStream) as T;
        }

        public static TConfig GetDefaultConfiguration<TConfig>() where TConfig : PluginConfiguration
        {
            TConfig result = Activator.CreateInstance(typeof(TConfig), true) as TConfig;

            if (result == null)
                throw new ConfigurationException();

            result.InitializeDefaultConfiguration();
            return result;
        }

        #endregion

        #endregion
    }
}
