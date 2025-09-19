using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
	private readonly string _path;
	private static CSVDatabase<T> instance = null;

	private CSVDatabase()
	{
		_path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..","..","..", "data", "chirp.cli.db.csv");;
	}

	public static CSVDatabase<T> Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new CSVDatabase<T>();
			}

			return instance;
		}
	}

    public IEnumerable<T> Read(int? limit = null)
    {
        if (!File.Exists(_path)) yield break;
        
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };
        
        using var reader = new StreamReader(_path);
        using var csv = new CsvReader(reader, config);
	
	
		var records = csv.GetRecords<T>();

		if (limit is null)
		{
			foreach (var record in records)
				yield return record;
		}
		else
		{
			foreach (var record in records.Take(limit.Value))
				yield return record;
		}
    }

    public void Store(T record)
    {
        // TODO: implement storing
        var dir = Path.GetDirectoryName(_path)!;
        Directory.CreateDirectory(dir);
        
        var fileExists = File.Exists(_path);
        
        using var stream = new FileStream(_path, FileMode.Append, FileAccess.Write, FileShare.Read);
        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        if (!fileExists)
        {
	        csv.WriteHeader<T>();
	        csv.NextRecord();
        }
        
        csv.WriteRecord(record);
        csv.NextRecord();
    }
}
