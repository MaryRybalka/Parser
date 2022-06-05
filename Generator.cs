namespace parser;

public class Generator
{
    private string filename = "result.cpp";

    void InitializeparentDirName()
    {
    }

    void GenerateStartMain(StreamWriter s)
    {
        s.WriteLine("int main(){");
    }

    void GenerateEndMain(StreamWriter s)
    {
        s.WriteLine("return 0;");
        s.WriteLine("}");
    }

    public void Generate()
    {
        string dirName = AppDomain.CurrentDomain.BaseDirectory;
        FileInfo fileInfo = new FileInfo(dirName);
        DirectoryInfo parentDir = fileInfo.Directory.Parent;
        string parentDirName = parentDir!.FullName;
        parentDirName = parentDirName.Remove(parentDirName.Length - 9, 9) + filename;

        StreamWriter sr = new StreamWriter(parentDirName);

        GenerateStartMain(sr);

        GenerateEndMain(sr);

        sr.Close();
    }
}