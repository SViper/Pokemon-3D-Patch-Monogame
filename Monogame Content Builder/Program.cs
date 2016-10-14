using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Monogame_Content_Builder
{
    public class Program
    {
        private static void Main(string[] args)
        {
            string AppDirectory = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);

            List<string> IncludedFolder = new List<string>() { "content", "maps", "scripts" };
            List<string> IncludedExtension = new List<string>() { ".dat", ".poke", ".trainer" };
            List<string> FinalResult = new List<string>();
            long MeasuredSize = 0;

            Console.WriteLine("Generating Content.mgcb");

            using (StreamWriter Writer = new StreamWriter(AppDirectory + "/Content.mgcb", false, Encoding.UTF8))
            {
                Writer.WriteLine();
                Writer.WriteLine("#----------------------------- Global Properties ----------------------------#");
                Writer.WriteLine();
                Writer.WriteLine("/outputDir:bin");
                Writer.WriteLine("/intermediateDir:obj");
                Writer.WriteLine("/platform:DesktopGL");
                Writer.WriteLine("/config:");
                Writer.WriteLine("/profile:Reach");
                Writer.WriteLine("/compress:True");
                Writer.WriteLine();
                Writer.WriteLine("#-------------------------------- References --------------------------------#");
                Writer.WriteLine();
                Writer.WriteLine();
                Writer.WriteLine("#---------------------------------- Content ---------------------------------#");
                Writer.WriteLine();

                foreach (string file in Directory.GetFiles(AppDirectory, "*.*", SearchOption.AllDirectories))
                {
                    // Normailize all file paths.
                    string TempFile = file.Replace(AppDirectory, "").Replace("\\", "/");

                    // Global Ignore
                    if (TempFile.ToLower().StartsWith("bin") || TempFile.ToLower().StartsWith("obj") || (TempFile.ToLower().StartsWith("content/models/") && !(Path.GetExtension(file).ToLower() == ".x" || Path.GetExtension(file).ToLower() == ".fbx")))
                        continue;

                    // Normal Files
                    if (Path.GetExtension(file).ToLower() == ".dat" || Path.GetExtension(file).ToLower() == ".poke" || Path.GetExtension(file).ToLower() == ".trainer" || Path.GetFileName(file).ToLower() == "meta")
                    {
                        Writer.WriteLine("#begin " + TempFile);
                        Writer.WriteLine("/copy:" + TempFile);
                        Writer.WriteLine();

                        if (!TempFile.ToLower().StartsWith("content/localization"))
                        {
                            foreach (string IncludeFolderTest in IncludedFolder)
                            {
                                if (TempFile.ToLower().StartsWith(IncludeFolderTest) && IncludedExtension.Contains(Path.GetExtension(file).ToLower()))
                                {
                                    MeasuredSize += new FileInfo(file).Length;
                                    string Hash = ToMD5(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                                    FinalResult.Add(file.Replace(AppDirectory, "").Replace("/", "\\") + ":" + Hash);
                                }
                            }
                        }
                    }

                    // Effect Importer - MonoGame
                    if (Path.GetExtension(file).ToLower() == ".fx")
                    {
                        Writer.WriteLine("#begin " + TempFile);
                        Writer.WriteLine("/importer:EffectImporter");
                        Writer.WriteLine("/processor:EffectProcessor");
                        Writer.WriteLine("/processorParam:DebugMode=Auto");
                        Writer.WriteLine("/build:" + TempFile);
                        Writer.WriteLine();
                    }

                    // Fbx Importer - MonoGame
                    if (Path.GetExtension(file).ToLower() == ".fbx")
                    {
                        Writer.WriteLine("#begin " + TempFile);
                        Writer.WriteLine("/importer:FbxImporter");
                        Writer.WriteLine("/processor:ModelProcessor");
                        Writer.WriteLine("/processorParam:ColorKeyColor=0,0,0,0");
                        Writer.WriteLine("/processorParam:ColorKeyEnabled=True");
                        Writer.WriteLine("/processorParam:DefaultEffect=BasicEffect");
                        Writer.WriteLine("/processorParam:GenerateMipmaps=True");
                        Writer.WriteLine("/processorParam:GenerateTangentFrames=False");
                        Writer.WriteLine("/processorParam:PremultiplyTextureAlpha=True");
                        Writer.WriteLine("/processorParam:PremultiplyVertexColors=True");
                        Writer.WriteLine("/processorParam:ResizeTexturesToPowerOfTwo=False");
                        Writer.WriteLine("/processorParam:RotationX=0");
                        Writer.WriteLine("/processorParam:RotationY=0");
                        Writer.WriteLine("/processorParam:RotationZ=0");
                        Writer.WriteLine("/processorParam:Scale=1");
                        Writer.WriteLine("/processorParam:SwapWindingOrder=False");
                        Writer.WriteLine("/processorParam:TextureFormat=Color");
                        Writer.WriteLine("/build:" + TempFile);
                        Writer.WriteLine();
                    }

                    // Sprite Font Importer - MonoGame
                    if (Path.GetExtension(file).ToLower() == ".spritefont")
                    {
                        Writer.WriteLine("#begin " + TempFile);
                        Writer.WriteLine("/importer:FontDescriptionImporter");
                        Writer.WriteLine("/processor:FontDescriptionProcessor");
                        Writer.WriteLine("/processorParam:TextureFormat=Compressed");
                        Writer.WriteLine("/build:" + TempFile);
                        Writer.WriteLine();
                    }

                    // H.264 Video - MonoGame
                    // Not implemented.

                    // Mp3 Importer - MonoGame
                    if (Path.GetExtension(file).ToLower() == ".mp3")
                    {
                        if (TempFile.ToLower().StartsWith("content/songs/"))
                        {
                            Writer.WriteLine("#begin " + TempFile);
                            Writer.WriteLine("/importer:Mp3Importer");
                            Writer.WriteLine("/processor:SongProcessor");
                            Writer.WriteLine("/processorParam:Quality=Low");
                            Writer.WriteLine("/build:" + TempFile);
                            Writer.WriteLine();
                        }
                        else if (TempFile.ToLower().StartsWith("content/sounds/"))
                        {
                            Writer.WriteLine("#begin " + TempFile);
                            Writer.WriteLine("/importer:Mp3Importer");
                            Writer.WriteLine("/processor:SoundEffectProcessor");
                            Writer.WriteLine("/processorParam:Quality=Low");
                            Writer.WriteLine("/build:" + TempFile);
                            Writer.WriteLine();
                        }
                    }

                    // Ogg Importer - MonoGame
                    if (Path.GetExtension(file).ToLower() == ".ogg")
                    {
                        if (TempFile.ToLower().StartsWith("content/songs/"))
                        {
                            Writer.WriteLine("#begin " + TempFile);
                            Writer.WriteLine("/importer:OggImporter");
                            Writer.WriteLine("/processor:SongProcessor");
                            Writer.WriteLine("/processorParam:Quality=Low");
                            Writer.WriteLine("/build:" + TempFile);
                            Writer.WriteLine();
                        }
                        else if (TempFile.ToLower().StartsWith("content/sounds/"))
                        {
                            Writer.WriteLine("#begin " + TempFile);
                            Writer.WriteLine("/importer:OggImporter");
                            Writer.WriteLine("/processor:SoundEffectProcessor");
                            Writer.WriteLine("/processorParam:Quality=Low");
                            Writer.WriteLine("/build:" + TempFile);
                            Writer.WriteLine();
                        }
                    }

                    // Open Asset Import Library - MonoGame
                    // Not implemented.

                    // Texture Importer - MonoGame
                    if (Path.GetExtension(file).ToLower() == ".png" || Path.GetExtension(file).ToLower() == ".jpg" || Path.GetExtension(file).ToLower() == ".bmp")
                    {
                        if (TempFile.ToLower().StartsWith("content/fonts/"))
                        {
                            Writer.WriteLine("#begin " + TempFile);
                            Writer.WriteLine("/importer:TextureImporter");
                            Writer.WriteLine("/processor:FontTextureProcessor");
                            Writer.WriteLine("/processorParam:FirstCharacter= ");
                            Writer.WriteLine("/processorParam:PremultiplyAlpha=True");
                            Writer.WriteLine("/processorParam:TextureFormat=Color");
                            Writer.WriteLine("/build:" + TempFile);
                            Writer.WriteLine();
                        }
                        else
                        {
                            Writer.WriteLine("#begin " + TempFile);
                            Writer.WriteLine("/importer:TextureImporter");
                            Writer.WriteLine("/processor:TextureProcessor");
                            Writer.WriteLine("/processorParam:ColorKeyColor=255,0,255,255");
                            Writer.WriteLine("/processorParam:ColorKeyEnabled=True");
                            Writer.WriteLine("/processorParam:GenerateMipmaps=False");
                            Writer.WriteLine("/processorParam:PremultiplyAlpha=True");
                            Writer.WriteLine("/processorParam:ResizeToPowerOfTwo=False");
                            Writer.WriteLine("/processorParam:MakeSquare=False");
                            Writer.WriteLine("/processorParam:TextureFormat=Color");
                            Writer.WriteLine("/build:" + TempFile);
                            Writer.WriteLine();
                        }
                    }

                    // Wav Importer - MonoGame
                    if (Path.GetExtension(file).ToLower() == ".wav")
                    {
                        if (TempFile.ToLower().StartsWith("content/songs/"))
                        {
                            Writer.WriteLine("#begin " + TempFile);
                            Writer.WriteLine("/importer:WavImporter");
                            Writer.WriteLine("/processor:SongProcessor");
                            Writer.WriteLine("/processorParam:Quality=Low");
                            Writer.WriteLine("/build:" + TempFile);
                            Writer.WriteLine();
                        }
                        else if (TempFile.ToLower().StartsWith("content/sounds/"))
                        {
                            Writer.WriteLine("#begin " + TempFile);
                            Writer.WriteLine("/importer:WavImporter");
                            Writer.WriteLine("/processor:SoundEffectProcessor");
                            Writer.WriteLine("/processorParam:Quality=Low");
                            Writer.WriteLine("/build:" + TempFile);
                            Writer.WriteLine();
                        }
                    }

                    // Wma Importer - MonoGame
                    if (Path.GetExtension(file).ToLower() == ".wma")
                    {
                        if (TempFile.ToLower().StartsWith("content/songs/"))
                        {
                            Writer.WriteLine("#begin " + TempFile);
                            Writer.WriteLine("/importer:WmaImporter");
                            Writer.WriteLine("/processor:SongProcessor");
                            Writer.WriteLine("/processorParam:Quality=Low");
                            Writer.WriteLine("/build:" + TempFile);
                            Writer.WriteLine();
                        }
                        else if (TempFile.ToLower().StartsWith("content/sounds/"))
                        {
                            Writer.WriteLine("#begin " + TempFile);
                            Writer.WriteLine("/importer:WmaImporter");
                            Writer.WriteLine("/processor:SoundEffectProcessor");
                            Writer.WriteLine("/processorParam:Quality=Low");
                            Writer.WriteLine("/build:" + TempFile);
                            Writer.WriteLine();
                        }
                    }

                    // Wmv Importer - MonoGame
                    // Not implemented.

                    // X Importer - MonoGame
                    if (Path.GetExtension(file).ToLower() == ".x")
                    {
                        Writer.WriteLine("#begin " + TempFile);
                        Writer.WriteLine("/importer:XImporter");
                        Writer.WriteLine("/processor:ModelProcessor");
                        Writer.WriteLine("/processorParam:ColorKeyColor=0,0,0,0");
                        Writer.WriteLine("/processorParam:ColorKeyEnabled=True");
                        Writer.WriteLine("/processorParam:DefaultEffect=BasicEffect");
                        Writer.WriteLine("/processorParam:GenerateMipmaps=True");
                        Writer.WriteLine("/processorParam:GenerateTangentFrames=False");
                        Writer.WriteLine("/processorParam:PremultiplyTextureAlpha=True");
                        Writer.WriteLine("/processorParam:PremultiplyVertexColors=True");
                        Writer.WriteLine("/processorParam:ResizeTexturesToPowerOfTwo=False");
                        Writer.WriteLine("/processorParam:RotationX=0");
                        Writer.WriteLine("/processorParam:RotationY=0");
                        Writer.WriteLine("/processorParam:RotationZ=0");
                        Writer.WriteLine("/processorParam:Scale=1");
                        Writer.WriteLine("/processorParam:SwapWindingOrder=False");
                        Writer.WriteLine("/processorParam:TextureFormat=Color");
                        Writer.WriteLine("/build:" + TempFile);
                        Writer.WriteLine();
                    }

                    // Xml Importer - MonoGame
                    // Not implemented.
                }
            }

            Console.WriteLine("Generating meta");

            File.WriteAllText(AppDirectory + "/meta", string.Join(",", FinalResult));

            string MetaHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(ToMD5(new FileStream(AppDirectory + "/meta", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))));
            Console.WriteLine("FileValidation.vb: Meta created! Expected Size: " + MeasuredSize + " | MetaHash: " + MetaHash);

            string vbNetFile = $@"Namespace Security

    Public Class FileValidation

        Shared _validated As Boolean = False
        Shared _valid As Boolean = False

        Const RUNVALIDATION As Boolean = False
        Const EXPECTEDSIZE As Integer = {MeasuredSize}
        Const METAHASH As String = ""{MetaHash}""

        Public Shared ReadOnly Property IsValid(ByVal ForceResult As Boolean) As Boolean
            Get
                If _validated = False Then
                    _validated = True
                    _valid = FilesValid()
                End If

                If GameController.IS_DEBUG_ACTIVE = True And ForceResult = False Then
                    Return True
                Else
                    Return _valid
                End If
            End Get
        End Property

        Private Shared Function FilesValid() As Boolean
            Dim MeasuredSize As Long = 0

            Dim files As New List(Of String)
            Dim paths() As String = {{""Content"", ""maps"", ""Scripts""}}
            Dim includeExt() As String = {{"".dat"", "".poke"", "".lua"", "".trainer""}}

            If RUNVALIDATION = True Then
                Logger.Log(Logger.LogTypes.Debug, ""FileValidation.vb: WARNING! FILE VALIDATION IS RUNNING!"")
                For Each subFolder As String In paths
                    For Each file As String In System.IO.Directory.GetFiles(GameController.GamePath & ""\"" & subFolder, ""*.*"", IO.SearchOption.AllDirectories)
                        If file.Contains(""\Content\Localization\"") = False Then
                            Dim ext As String = System.IO.Path.GetExtension(file)
                            If includeExt.Contains(ext.ToLower()) = True Then
                                files.Add(file.Remove(0, GameController.GamePath.Length + 1))
                            End If
                        End If
                    Next
                Next

                Dim s As String = """"
                For Each f As String In files
                    Dim i As Long = New System.IO.FileInfo(GameController.GamePath & ""\"" & f).Length
                    Dim hash As String = GetMD5FromFile(GameController.GamePath & ""\"" & f)

                    FileDictionary.Add((GameController.GamePath & ""\"" & f).ToLower(), New ValidationStorage(i, hash))
                    MeasuredSize += i

                    If s <> """" Then
                        s &= "",""
                    End If
                    s &= f & "":"" & hash
                Next

                System.IO.File.WriteAllText(GameController.GamePath & ""\meta"", s)
                Logger.Log(Logger.LogTypes.Debug, ""FileValidation.vb: Meta created! Expected Size: "" & MeasuredSize &
                           ""|MetaHash: "" & StringObfuscation.Obfuscate(GetMD5FromFile(GameController.GamePath & ""\meta"")))

                Core.GameInstance.Exit()
            Else
                If System.IO.File.Exists(GameController.GamePath & ""\meta"") = True Then
                    If GetMD5FromFile(GameController.GamePath & ""\meta"") = StringObfuscation.DeObfuscate(METAHASH) Then
                        files = System.IO.File.ReadAllText(GameController.GamePath & ""\meta"").Split(CChar("","")).ToList()
                        Logger.Debug(""Meta loaded. Files to check: "" & files.Count)
                    Else
                        Logger.Log(Logger.LogTypes.Warning, ""FileValidation.vb: Failed to load Meta (Hash incorrect)! File Validation will fail!"")
                    End If
                Else
                    Logger.Log(Logger.LogTypes.Warning, ""FileValidation.vb: Failed to load Meta (File not found)! File Validation will fail!"")
                End If

                For Each f As String In files
                    Dim fileName As String = f.Split(CChar("":""))(0)
                    Dim fileHash As String = f.Split(CChar("":""))(1)

                    If System.IO.File.Exists(GameController.GamePath & ""\"" & fileName) Then
                        Dim i As Long = New System.IO.FileInfo(GameController.GamePath & ""\"" & fileName).Length
                        FileDictionary.Add((GameController.GamePath & ""\"" & fileName).ToLower(), New ValidationStorage(i, fileHash))
                        MeasuredSize += i
                    End If
                Next
            End If

            If MeasuredSize = EXPECTEDSIZE Then
                Return True
            End If
            Return False
        End Function

        Shared FileDictionary As New Dictionary(Of String, ValidationStorage)

        Public Shared Sub CheckFileValid(ByVal file As String, ByVal relativePath As Boolean, ByVal origin As String)
            Dim validationResult As String = ValidateFile(file, relativePath)
            If validationResult <> """" Then
                Logger.Log(Logger.LogTypes.ErrorMessage, ""FileValidation.vb: Detected invalid files in a sensitive game environment. Stopping execution..."")

                Dim ex As New Exception(""The File Validation system detected invalid files in a sensitive game environment."")
                ex.Data.Add(""File"", file)
                ex.Data.Add(""File Validation result"", validationResult)
                ex.Data.Add(""Call Origin"", origin)
                ex.Data.Add(""Relative Path"", relativePath)

                Throw ex
            End If
        End Sub

        Private Shared Function ValidateFile(ByVal file As String, ByVal relativePath As Boolean) As String
            If Core.Player.IsGamejoltSave = True And GameController.IS_DEBUG_ACTIVE = False Then
                Dim filePath As String = file.Replace(""/"", ""\"")
                If relativePath = True Then
                    filePath = GameController.GamePath & ""\"" & file
                End If
                Dim i As Long = New System.IO.FileInfo(filePath).Length

                If System.IO.File.Exists(filePath) = True Then
                    If FileDictionary.ContainsKey(filePath.ToLower()) = True Then
                        If i <> FileDictionary(filePath.ToLower()).FileSize Then
                            Return ""File Validation rendered the file invalid. Array length invalid.""
                        Else
                            Dim hash As String = GetMD5FromFile(filePath)
                            If hash <> FileDictionary(filePath.ToLower()).Hash Then
                                Return ""File Validation rendered the file invalid. File has been edited.""
                            End If
                        End If
                    Else
                        Return ""The File Validation system couldn't find the requested file.""
                    End If
                End If
            End If
            Return """"
        End Function

        Private Shared Function GetMD5FromFile(ByVal file As String) As String
            Dim MD5 As System.Security.Cryptography.MD5 = System.Security.Cryptography.MD5.Create()
            Dim Hash As Byte()
            Dim sb As New System.Text.StringBuilder()

            Using st As New IO.FileStream(file, IO.FileMode.Open, IO.FileAccess.Read)
                Hash = MD5.ComputeHash(st)
            End Using

            For Each b In Hash
                sb.Append(b.ToString(""X2""))
            Next

            Return sb.ToString()
        End Function

        Private Class ValidationStorage

            Public Hash As String = """"
            Public FileSize As Long = 0

            Public Sub New(ByVal FileSize As Long, ByVal Hash As String)
                Me.FileSize = FileSize
                Me.Hash = Hash
            End Sub

            Public Function CheckValidation(ByVal FileSize As Long) As Boolean
                Return (FileSize = Me.FileSize)
            End Function

            Public Function CheckValidation(ByVal FileSize As Long, ByVal Hash As String) As Boolean
                If Me.FileSize = FileSize And Me.Hash = Hash Then
                    Return True
                End If
                Return False
            End Function

        End Class

    End Class

End Namespace";

            Console.WriteLine("Generating FileValidation.vb");

            File.WriteAllText(AppDirectory + "..\\2.5DHero\\Security\\FileValidation.vb", vbNetFile, Encoding.UTF8);

            Environment.Exit(0);
        }

        /// <summary>
        /// Convert a <see cref="Stream"/> to <see cref="MD5"/> checksum.
        /// </summary>
        /// <param name="Stream">Stream to convert.</param>
        public static string ToMD5(Stream Stream)
        {
            try
            {
                Stream.Seek(0, SeekOrigin.Begin);
                using (MD5 md5 = MD5.Create())
                {
                    byte[] hash = md5.ComputeHash(Stream);
                    return string.Join("", hash.Select(a => a.ToString("X2")).ToArray());
                }
            }
            catch (Exception) { return null; }
        }
    }
}
