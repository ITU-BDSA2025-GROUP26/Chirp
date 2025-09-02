string path = @"C:\Users\nanhe\chirp\Chirp.CLI\chirp_cli_db.csv";


if (File.Exists(path))
{
    StreamReader sr = new StreamReader(File.OpenRead(path));

    while (!sr.EndOfStream)
    {
        var line = sr.ReadLine();
        var values = line.Split(',');
        string user = null;
        string mes = null;
        string time = null;
        if (values.Length > 3)
        {
            user = values[0];
            mes = values[1] + values[2];
            time = values[3];
        }
        else
        {
            user = values[0];
            mes = values[1];
            time = values[2];
        }
        if (!(user == "Author"))
        {
            Console.WriteLine(user + " @ " + time + ": " + mes);
        }
    }

    sr.Close();
}
else
{
    Console.WriteLine("Files does not exist");
}