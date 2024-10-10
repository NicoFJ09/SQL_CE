using Entities;
using LiteDB;
using Trees;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace StoreDataManager
{
    public class Store
    {
        private static Store? instance = null;
        private static readonly object _lock = new object();
        private static readonly string BaseDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\StorageFolder"));
        public static string DatabasesPath = Path.Combine(BaseDirectory, "Databases");
        public static string CatalogPath = Path.Combine(BaseDirectory, "Catalog");
        private SystemCatalog systemCatalog;


//===============================PRESETS===============================
        private Store()
        {
            // Inicializar systemCatalog en el constructor
            systemCatalog = new SystemCatalog(CatalogPath);
            InitializeSystemCatalog();
        }

        public static Store GetInstance()
        {
            lock (_lock)
            {
                if (instance == null)
                {
                    instance = new Store();
                }
                return instance;
            }
        }

        private void InitializeSystemCatalog()
        {
            // Verificar el directorio base
            Console.WriteLine($"Directorio base: {BaseDirectory}");

            // Asegurarse de que el catálogo del sistema y la carpeta superior existan al inicializar
            try
            {
                if (!Directory.Exists(CatalogPath))
                {
                    Console.WriteLine($"El directorio {CatalogPath} no existe. Creando directorio...");
                    Directory.CreateDirectory(CatalogPath);
                    Console.WriteLine($"Directorio {CatalogPath} creado exitosamente.");
                }
                else
                {
                    Console.WriteLine($"El directorio {CatalogPath} ya existe.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear el directorio {CatalogPath}: {ex.Message}");
            }
        }


//===============================DB METHODS===============================
        public OperationStatus CreateDatabase(string dbName)
        {
            bool worked = DatabaseOperations.CreateDatabase(systemCatalog, DatabasesPath, dbName);
            if (worked)
                return OperationStatus.Success;
            else
                return OperationStatus.Error;
        }

        public OperationStatus SetDatabase(string dbName) 
        {
            bool worked = DatabaseOperations.SetDatabase(dbName);
            if (worked)
                return OperationStatus.Success;
            else
                return OperationStatus.Error;
        }


        public OperationStatus CreateTable(string tableName, List<string> columns, string treeType)
        {
            bool worked = DatabaseOperations.CreateTable(systemCatalog, tableName, columns, treeType, DatabasesPath);
            if (worked)
                return OperationStatus.Success;
            else
                return OperationStatus.Error;
        }

        public OperationStatus IndexColumn(string tableName, string column)
        {
            bool worked = DatabaseOperations.IndexColumn(systemCatalog, tableName, column, DatabasesPath);
            if (worked)
                return OperationStatus.Success;
            else
                return OperationStatus.Error;
        }
        public OperationStatus Select()
        {
            // // Creates a default Table called ESTUDIANTES
            // var tablePath = $@"{DataPath}\TESTDB\ESTUDIANTES.Table";
            // using (FileStream stream = File.Open(tablePath, FileMode.OpenOrCreate))
            // using (BinaryReader reader = new (stream))
            // {
            //     // Print the values as a I know exactly the types, but this needs to be done right
            //     Console.WriteLine(reader.ReadInt32());
            //     Console.WriteLine(reader.ReadString());
            //     Console.WriteLine(reader.ReadString());
            //     return OperationStatus.Success;
            // }
            Console.WriteLine("SELECT TABLE PROPERLY");
            //Console.WriteLine($"Select from table: {tableName}");
            return OperationStatus.Success;
        }
    }
}
