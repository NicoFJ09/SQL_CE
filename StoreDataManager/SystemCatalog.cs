using System;
using System.Collections.Generic;
using System.IO;

public class SystemCatalog
{
    private string catalogPath;

    public SystemCatalog(string catalogPath)
    {
        this.catalogPath = catalogPath;
        InitializeCatalog();
    }

//===============================DATABASE LEVEL===============================
    private void InitializeCatalog()
    {
        if (!Directory.Exists(catalogPath))
        {
            Directory.CreateDirectory(catalogPath);
        }

        CreateFileIfNotExists(Path.Combine(catalogPath, "SystemDatabases"));
    }

    private void CreateFileIfNotExists(string filePath)
    {
        string directoryPath = Path.GetDirectoryName(filePath) ?? string.Empty;

        if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
        }
    }

    public List<string> ListDatabases()
    {
        string filePath = Path.Combine(catalogPath, "SystemDatabases");
        if (!File.Exists(filePath))
        {
            return new List<string>();
        }
        return new List<string>(File.ReadAllLines(filePath));
    }

    public void AddDatabase(string databaseName)
    {
        string filePath = Path.Combine(catalogPath, "SystemDatabases");
        if (!File.Exists(filePath))
        {
            return;
        }
        File.AppendAllText(filePath, databaseName + Environment.NewLine);

        // Crear una carpeta para la base de datos
        string dbCatalogPath = Path.Combine(catalogPath, databaseName);
        if (!Directory.Exists(dbCatalogPath))
        {
            Directory.CreateDirectory(dbCatalogPath);
        }

        // Crear el archivo de tablas dentro de la carpeta de la base de datos
        CreateFileIfNotExists(Path.Combine(dbCatalogPath, "Tables"));
    }


//===============================TABLE LEVEL===============================
    public void AddTable(string databaseName, string tableName, List<string> columns, string treeType)
    {
        string dbCatalogPath = Path.Combine(catalogPath, databaseName);
        string filePath = Path.Combine(dbCatalogPath, "Tables");
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Dato inexistente. Intente de nuevo.");
            return;
        }
        File.AppendAllText(filePath, $"{tableName} {treeType}" + Environment.NewLine);

        // Crear una carpeta para la tabla
        string tableCatalogPath = Path.Combine(dbCatalogPath, tableName);
        if (!Directory.Exists(tableCatalogPath))
        {
            Directory.CreateDirectory(tableCatalogPath);
        }

        // Crear el archivo de columnas dentro de la carpeta de la tabla
        string columnsFilePath = Path.Combine(tableCatalogPath, "Columns");
        CreateFileIfNotExists(columnsFilePath);

        // Añadir las columnas al archivo de columnas
        File.WriteAllLines(columnsFilePath, columns);
    }

    public void RemoveTable(string dbName, string tableName)
    {
        // Ruta del archivo de tablas dentro de la carpeta específica de la base de datos
        string dbCatalogPath = Path.Combine(catalogPath, dbName);
        string filePath = Path.Combine(dbCatalogPath, "Tables");

        // Eliminar el historial de la tabla del archivo de tablas
        if (File.Exists(filePath))
        {
            var lines = File.ReadAllLines(filePath).Where(line => !line.Contains(tableName)).ToList();
            File.WriteAllLines(filePath, lines);
            Console.WriteLine($"Historial de la tabla {tableName} eliminado del archivo de tablas.");
        }
        else
        {
            Console.WriteLine($"El archivo {filePath} no existe.");
        }

        // Eliminar la carpeta de la tabla
        string tableCatalogPath = Path.Combine(dbCatalogPath, tableName);
        if (Directory.Exists(tableCatalogPath))
        {
            try
            {
                Directory.Delete(tableCatalogPath, true);
                Console.WriteLine($"Carpeta de la tabla {tableName} eliminada.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar la carpeta de la tabla {tableName}: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"La carpeta {tableCatalogPath} no existe.");
        }
    }
    public string? GetTreeType(string databaseName, string tableName)
    {
        string dbCatalogPath = Path.Combine(catalogPath, databaseName);
        string filePath = Path.Combine(dbCatalogPath, "Tables");

        if (!File.Exists(filePath))
        {
            Console.WriteLine("El archivo de tablas no existe.");
            return null;
        }

        var lines = File.ReadAllLines(filePath);
        foreach (var line in lines)
        {
            var parts = line.Split(' ');
            if (parts.Length == 2 && parts[0].Equals(tableName, StringComparison.OrdinalIgnoreCase))
            {
                return parts[1];
            }
        }

        return null;
    }


//===============================COLUMN LEVEL===============================

    public List<string> GetColumns(string databaseName, string tableName)
    {
        string filePath = Path.Combine(catalogPath, databaseName, tableName, "Columns");
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Dato inexistente. Intente de nuevo.");
            return new List<string>();
        }

        var lines = File.ReadAllLines(filePath);
        var columns = lines.Select(line => line.Split(' ')[0]).ToList();
        return columns;
    }
    public void IndexColumn(string databaseName, string tableName, string columnName)
    {
        string filePath = Path.Combine(catalogPath, databaseName, tableName, "Columns");
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Dato inexistente. Intente de nuevo.");
            return;
        }
        var columns = new List<string>(File.ReadAllLines(filePath));

        for (int i = 0; i < columns.Count; i++)
        {
            if (columns[i].Equals(columnName, StringComparison.OrdinalIgnoreCase))
            {
                columns[i] = $"{columnName} INDEXED";
                File.WriteAllLines(filePath, columns);
                return;
            }
        }

        Console.WriteLine($"Columna '{columnName}' no encontrada en la tabla '{tableName}' de la base de datos '{databaseName}'.");
    }

    public string? GetIndexedColumn(string databaseName, string tableName)
    {
        string filePath = Path.Combine(catalogPath, databaseName, tableName, "Columns");
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Dato inexistente. Intente de nuevo.");
            return null;
        }
        var columns = new List<string>(File.ReadAllLines(filePath));

        foreach (var column in columns)
        {
            if (column.EndsWith("INDEXED", StringComparison.OrdinalIgnoreCase))
            {
                // Cortar la parte "INDEXED" y devolver solo el nombre de la columna
                return column.Split(' ')[0];
            }
        }

        return null;
    }

    public (string DataType, int? Length) GetDataType(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return (string.Empty, null);
        }

        var parts = input.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2)
        {
            throw new ArgumentException("El formato de entrada es incorrecto. Se esperaba 'NAME-DATATYPE(LENGTH)'.");
        }

        string dataTypePart = parts[1].Trim().ToUpper();
        string dataType = dataTypePart;
        int? length = null;

        if (dataTypePart.StartsWith("VARCHAR"))
        {
            int startIndex = dataTypePart.IndexOf('(');
            int endIndex = dataTypePart.IndexOf(')');
            if (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
            {
                string lengthStr = dataTypePart.Substring(startIndex + 1, endIndex - startIndex - 1);
                if (int.TryParse(lengthStr, out int parsedLength))
                {
                    length = parsedLength;
                }
            }
            dataType = "VARCHAR"; // Normalizar el tipo de dato a "VARCHAR"
        }

        return (dataType, length);
    }
    public bool IsDataType(string type, int? length, string value)
    {
        switch (type)
        {
            case "INTEGER":
                return int.TryParse(value, out _);
            case "DOUBLE":
                return double.TryParse(value, out _);
            case "VARCHAR":
                return length.HasValue ? value.Length <= length.Value : true;
            case "DATETIME":
                return DateTime.TryParse(value, out _);
            default:
                return false; // Tipo de dato no reconocido
        }
    }
}