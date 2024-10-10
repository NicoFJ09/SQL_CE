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

        private Store()
        {
            // Imprimir las rutas para verificar que son correctas
            Console.WriteLine($"BaseDirectory: {BaseDirectory}");
            Console.WriteLine($"DatabasesPath: {DatabasesPath}");
            Console.WriteLine($"CatalogPath: {CatalogPath}");

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

        public void CreateDatabase(string dbName)
        {
            // Verificar el directorio base
            Console.WriteLine($"Directorio base: {BaseDirectory}");

            try
            {
                if (!Directory.Exists(DatabasesPath))
                {
                    Console.WriteLine($"El directorio {DatabasesPath} no existe. Creando directorio...");
                    Directory.CreateDirectory(DatabasesPath);
                    Console.WriteLine($"Directorio {DatabasesPath} creado exitosamente.");
                }
                else
                {
                    Console.WriteLine($"El directorio {DatabasesPath} ya existe.");
                }

                string dbPath = Path.Combine(DatabasesPath, $"{dbName}.db");

                if (File.Exists(dbPath))
                {
                    Console.WriteLine("La base de datos ya existe. Intente con otro nombre.");
                    return;
                }

                // Crear la base de datos usando LiteDB
                using (var db = new LiteDatabase(dbPath))
                {
                    Console.WriteLine($"Base de datos '{dbName}' creada en {dbPath}");
                }

                // Actualizar el SystemCatalog
                systemCatalog.AddDatabase(dbName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear la base de datos: {ex.Message}");
            }
        }

        public OperationStatus CreateTable(string tableName)
        {
            Console.WriteLine("CREATE TABLE PROPERLY");
            Console.WriteLine($"Table Name {tableName}");
            // // Creates a default DB called TESTDB
            // Directory.CreateDirectory($@"{DataPath}\TESTDB");

            // // Creates a default Table called ESTUDIANTES
            // var tablePath = $@"{DataPath}\TESTDB\ESTUDIANTES.Table";

            // using (FileStream stream = File.Open(tablePath, FileMode.OpenOrCreate))
            // using (BinaryWriter writer = new (stream))
            // {
            //     // Create an object with a hardcoded.
            //     // First field is an int, second field is a string of size 30,
            //     // third is a string of 50
            //     int id = 1;
            //     string nombre = "Isaac".PadRight(30); // Pad to make the size of the string fixed
            //     string apellido = "Ramirez".PadRight(50);
                
            //     writer.Write(id);
            //     writer.Write(nombre);
            //     writer.Write(apellido);
            // }
            return OperationStatus.Success;
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
