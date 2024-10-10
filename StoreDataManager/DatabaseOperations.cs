using LiteDB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Trees;
public class DatabaseOperations
{

    // Diccionario para mantener un registro de las columnas indexadas y sus árboles asociados
    public static string currentDbName = string.Empty;
    private static Dictionary<string, ColumnIndex> indexedColumns = new Dictionary<string, ColumnIndex>();


//===============================DATABASE METHODS ===============================


    public static bool CreateDatabase(SystemCatalog systemCatalog, string DatabasesPath, string dbName)
    {
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
                return false; // Indicador de fracaso
            }

            // Crear la base de datos usando LiteDB
            using (var db = new LiteDatabase(dbPath))
            {
                Console.WriteLine($"Base de datos '{dbName}' creada en {dbPath}");
            }

            // Actualizar el SystemCatalog
            systemCatalog.AddDatabase(dbName);
            return true; // Indicador de éxito
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al crear la base de datos: {ex.Message}");
            return false; // Indicador de fracaso en caso de excepción
        }
    }

    public static bool SetDatabase(string dbName)
    {
        if (string.IsNullOrWhiteSpace(dbName))
        {
            Console.WriteLine("Nombre de base de datos inválido. Intente de nuevo.");
            return false;
        }
        currentDbName = dbName;
        return true;
    }

    public static bool CreateTable(SystemCatalog systemCatalog, string tableName, List<string> columns, string treeType, string DatabasesPath)
    {
        if (string.IsNullOrWhiteSpace(currentDbName))
        {
            Console.WriteLine("Nombre de base de datos inválido. Intente de nuevo.");
            return false;
        }

        string dbPath = Path.Combine(DatabasesPath, $"{currentDbName}.db");

        if (!File.Exists(dbPath))
        {
            Console.WriteLine($"La base de datos '{currentDbName}' no existe.");
            return false;
        }

        using (var db = new LiteDatabase(dbPath))
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                Console.WriteLine("Nombre de tabla inválido. Intente de nuevo.");
                return false;
            }

            if (columns == null || columns.Count == 0)
            {
                Console.WriteLine("No se ingresaron columnas. Operación cancelada.");
                return false;
            }

            // Crear la tabla y añadir las columnas en LiteDB
            var col = db.GetCollection<BsonDocument>(tableName);
            var doc = new BsonDocument();
        foreach (var column in columns)
        {
            var columnParts = column.Split(' ');
            var columnName = columnParts[0]; // Obtener la primera parte antes del espacio
            doc[columnName] = string.Empty; // Inicializar con valores vacíos
            Console.WriteLine($"Añadido: {columnName}"); // Imprimir lo que se ha añadido
        }
        col.Insert(doc); // Insertar un documento inicial para crear la colección

        // Actualizar el catálogo del sistema
        systemCatalog.AddTable(currentDbName, tableName, columns, treeType);

        Console.WriteLine($"Tabla '{tableName}' creada en la base de datos '{currentDbName}' con columnas: {string.Join(", ", columns)}");
        }

        return true;
    }

    public static bool IndexColumn(SystemCatalog systemCatalog, string tableName, string columnName, string DatabasesPath)
{
    
    if (string.IsNullOrWhiteSpace(currentDbName))
    {
        Console.WriteLine("Nombre de base de datos inválido.");
        return false;
    }

    if (string.IsNullOrWhiteSpace(tableName))
    {
        Console.WriteLine("Nombre de tabla inválido.");
        return false;
    }

    var indexedColumnsInTable = indexedColumns.Keys.Where(key => key.StartsWith($"{currentDbName}.{tableName}.")).ToList();
    if (indexedColumnsInTable.Count > 0)
    {
        Console.WriteLine($"Error: Ya existe una columna indexada en la tabla '{tableName}'. Solo se permite una columna indexada por tabla.");
        return false;
    }

    if (string.IsNullOrWhiteSpace(columnName))
    {
        Console.WriteLine("Nombre de columna inválido.");
        return false;
    }

    // Obtener el tipo de árbol para la tabla
    string treeType = systemCatalog.GetTreeType(currentDbName, tableName) ?? string.Empty;
    Console.WriteLine($"El tipo de árbol es '{treeType}'.");
    if (string.IsNullOrWhiteSpace(treeType))
    {
        Console.WriteLine($"No se encontró el tipo de árbol para la tabla '{tableName}'.");
        return false;
    }

    ColumnIndex index = new ColumnIndex(treeType == "B-Tree" ? TreeType.BTree : TreeType.BST);

    // Indexar la columna
    string dbPath = Path.Combine(DatabasesPath, $"{currentDbName}.db");
    using (var db = new LiteDatabase(dbPath))
    {
        var col = db.GetCollection<BsonDocument>(tableName);

        foreach (var doc in col.FindAll())
        {
            if (doc.ContainsKey(columnName))
            {
                var value = doc[columnName].AsString;
                var id = doc["_id"].AsObjectId.ToString(); // Convertir ObjectId a cadena
                index.Insert(value, id);
            }
        }

        Console.WriteLine($"Columna '{columnName}' indexada con tipo de árbol '{treeType}'.");
    }

    // Guardar el índice en el diccionario
    string key = $"{currentDbName}.{tableName}.{columnName}";
    indexedColumns[key] = index;

    // Actualizar el SystemCatalog
    systemCatalog.IndexColumn(currentDbName, tableName, columnName);
    return true;
}

    public static bool InsertInto(SystemCatalog systemCatalog, string tableName, List<string> columns, List<string> values)
    {
        return true;
    }

    public static bool Select(SystemCatalog systemCatalog, string tableName, List<string> columns, string condition)
    {
        return true;
    }
    
    public static bool Update(SystemCatalog systemCatalog, string tableName, string column, string value, string condition)
    {
        return true;
    }

    public static bool Delete(SystemCatalog systemCatalog, string tableName, string condition)
    {
        return true;
    }

    private static List<BsonDocument> Where(string tableName, string columnName)
    {
        return null;
    }

    private static List<BsonDocument> OrderBy(List<BsonDocument> filteredDocuments, string columnName, string order)
    {
        return null;
    }
}