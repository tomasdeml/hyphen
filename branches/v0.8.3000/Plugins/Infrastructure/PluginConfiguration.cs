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
using System.Runtime.Serialization.Formatters.Binary;
using Virtuoso.Miranda.Plugins.Collections;
using Virtuoso.Miranda.Plugins.Configuration;
using Virtuoso.Miranda.Plugins.Resources;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    [Serializable]
    public abstract class PluginConfiguration
    {
        #region Fields

        private static readonly object SyncObject = new object();

        #endregion

        #region .ctors

        protected PluginConfiguration()
        {
            Values = new ConfigurationValues();
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

        public ConfigurationValues Values { get; private set; }

        public bool IsDirty
        {
            get; protected internal set;
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

            ConfigurationOptionsAttribute options;
            Type configAttribType = typeof(ConfigurationOptionsAttribute);

            if (configType.IsDefined(configAttribType, false))
                options = (ConfigurationOptionsAttribute)configType.GetCustomAttributes(configAttribType, false)[0];
            else
                options = new ConfigurationOptionsAttribute();

            return options.Initialize();
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

                    using (IStorage storage = (IStorage) Activator.CreateInstance(options.Storage))
                    {
                        if (options.Encrypt)
                            SerializeEncrypted(storage, options);
                        else
                            Serialize(storage, options);
                    }
                }
            }
            catch (Exception e)
            {
                throw new ConfigurationException(TextResources.ExceptionMsg_UnableToSaveConfiguration_StorageError, e);
            }
        }

        public void Delete()
        {
            Delete(GetType());
        }

        public static void Delete(Type configType)
        {
            try
            {
                lock (SyncObject)
                {
                    ConfigurationOptionsAttribute options = GetOptions(configType);

                    using (IStorage storage = (IStorage) Activator.CreateInstance(options.Storage))
                        storage.Delete(configType, options);
                }
            }
            catch (Exception)
            {
                // No need to handle
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
            IEncryption encryption = (IEncryption) Activator.CreateInstance(options.Encryption);

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

        public static TConfig Load<TConfig>() where TConfig : PluginConfiguration
        {
            return (TConfig) Load(typeof (TConfig));
        }

        public static PluginConfiguration Load(Type configType)
        {
            try
            {
                lock (SyncObject)
                {
                    ConfigurationOptionsAttribute options = GetOptions(configType);

                    using (IStorage storage = (IStorage) Activator.CreateInstance(options.Storage))
                    {
                        if (!storage.Exists(configType, options))
                            return GetDefaultConfiguration(configType);

                        using (Stream stream = storage.OpenRead(configType, options))
                        {
                            PluginConfiguration result = options.Encrypt ? DeserializeEncrypted(stream, options) : Deserialize(stream);
                            result.OnAfterDeserialization();

                            return result;
                        }
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                throw new ConfigurationException(TextResources.ExceptionMsg_UnableToLoadConfiguration_StorageError, e);                
            }
            catch (Exception e)
            {
                PluginConfiguration defaults = GetDefaultConfiguration(configType);
                defaults.Save();

                throw new ConfigurationException(TextResources.ExceptionMsg_UnableToLoadConfiguration_StorageError, e);
            }
        }

        public static bool Exists(Type configType)
        {
            try
            {
                lock (SyncObject)
                {
                    ConfigurationOptionsAttribute options = GetOptions(configType);

                    using (IStorage storage = (IStorage)Activator.CreateInstance(options.Storage))
                        return storage.Exists(configType, options);
                }
            }
            catch (Exception)
            {
                return false;
            } 
        }

        private static PluginConfiguration Deserialize(Stream stream)
        {
            return new BinaryFormatter().Deserialize(stream) as PluginConfiguration;
        }

        private static PluginConfiguration DeserializeEncrypted(Stream stream, ConfigurationOptionsAttribute options)
        {
            byte[] protectedData = FetchStream(stream);
            byte[] data = ((IEncryption) Activator.CreateInstance(options.Encryption)).Decrypt(protectedData);

            using (Stream serializedStream = new MemoryStream(data))
                return new BinaryFormatter().Deserialize(serializedStream) as PluginConfiguration;
        }

        public static TConfig GetDefaultConfiguration<TConfig>() where TConfig : PluginConfiguration
        {
            return (TConfig) GetDefaultConfiguration(typeof (TConfig));
        }

        public static PluginConfiguration GetDefaultConfiguration(Type configType)
        {
            PluginConfiguration result = Activator.CreateInstance(configType, true) as PluginConfiguration;

            if (result == null)
                throw new ConfigurationException();

            result.InitializeDefaultConfiguration();
            return result;
        }

        #endregion

        #endregion
    }
}
