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
            string Temp = null;

            Console.WriteLine("Generating BackdropShader.fx");

            using (StreamReader Reader = new StreamReader(AppDirectory + "Content/Effects/BackdropShader.fx"))
                Temp = Reader.ReadToEnd();

            Temp = Temp.Replace("vs_2_0", args[0] == "DesktopGL" ? "vs_2_0" : "vs_4_0");
            Temp = Temp.Replace("ps_2_0", args[0] == "DesktopGL" ? "ps_2_0" : "ps_4_0");

            using (StreamWriter Writer = new StreamWriter(AppDirectory + "Content/Effects/BackdropShader.fx", false, Encoding.UTF8))
            {
                foreach (char letter in Temp)
                    Writer.Write(letter);
            }

            Console.WriteLine("Generating BlurEffect.fx");

            using (StreamReader Reader = new StreamReader(AppDirectory + "Content/Effects/BlurEffect.fx"))
                Temp = Reader.ReadToEnd();

            Temp = Temp.Replace("vs_2_0", args[0] == "DesktopGL" ? "vs_2_0" : "vs_4_0");
            Temp = Temp.Replace("ps_2_0", args[0] == "DesktopGL" ? "ps_2_0" : "ps_4_0");

            using (StreamWriter Writer = new StreamWriter(AppDirectory + "Content/Effects/BlurEffect.fx", false, Encoding.UTF8))
            {
                foreach (char letter in Temp)
                    Writer.Write(letter);
            }

            Console.WriteLine("Generating DiffuseShader.fx");

            using (StreamReader Reader = new StreamReader(AppDirectory + "Content/Effects/DiffuseShader.fx"))
                Temp = Reader.ReadToEnd();

            Temp = Temp.Replace("vs_2_0", args[0] == "DesktopGL" ? "vs_2_0" : "vs_4_0");
            Temp = Temp.Replace("ps_2_0", args[0] == "DesktopGL" ? "ps_2_0" : "ps_4_0");

            using (StreamWriter Writer = new StreamWriter(AppDirectory + "Content/Effects/DiffuseShader.fx", false, Encoding.UTF8))
            {
                foreach (char letter in Temp)
                    Writer.Write(letter);
            }

            Console.WriteLine("Generating Toon.fx");

            using (StreamReader Reader = new StreamReader(AppDirectory + "Content/Effects/Toon.fx"))
                Temp = Reader.ReadToEnd();

            Temp = Temp.Replace("vs_1_1", args[0] == "DesktopGL" ? "vs_1_1" : "vs_4_0");
            Temp = Temp.Replace("vs_2_0", args[0] == "DesktopGL" ? "vs_2_0" : "vs_4_0");
            Temp = Temp.Replace("ps_2_0", args[0] == "DesktopGL" ? "ps_2_0" : "ps_4_0");

            using (StreamWriter Writer = new StreamWriter(AppDirectory + "Content/Effects/Toon.fx", false, Encoding.UTF8))
            {
                foreach (char letter in Temp)
                    Writer.Write(letter);
            }

            Console.WriteLine("Generating Content.mgcb");

            using (StreamWriter Writer = new StreamWriter(AppDirectory + "Content.mgcb", false, Encoding.UTF8))
            {
                Writer.WriteLine();
                Writer.WriteLine("#----------------------------- Global Properties ----------------------------#");
                Writer.WriteLine();
                Writer.WriteLine("/outputDir:bin");
                Writer.WriteLine("/intermediateDir:obj");
                Writer.WriteLine("/platform:" + args[0]);
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

            Temp = string.Join(",", FinalResult);

            using (StreamWriter Writer = new StreamWriter(AppDirectory + "meta", false, Encoding.UTF8))
            {
                foreach (char letter in Temp)
                    Writer.Write(letter);
            }

            string MetaHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(ToMD5(new FileStream(AppDirectory + "meta", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))));

            Console.WriteLine("FileValidation.vb: Meta created! Expected Size: " + MeasuredSize + " | MetaHash: " + MetaHash);

            Console.WriteLine("Generating FileValidation.vb");

            using (StreamReader Reader = new StreamReader(AppDirectory + "../2.5DHero/Security/FileValidation.vb"))
                Temp = Reader.ReadToEnd();

            string[] Temp2 = Temp.Split('\n');
            
            for (int i = 0; i < Temp2.Length; i++)
            {
                if (Temp2[i].Trim().Contains("Const EXPECTEDSIZE As Integer ="))
                    Temp2[i] = "        Const EXPECTEDSIZE As Integer = " + MeasuredSize.ToString();
                else if (Temp2[i].Trim().Contains("Const METAHASH As String ="))
                    Temp2[i] = "        Const METAHASH As String = \"" + MetaHash + "\"";
            }

            using (StreamWriter Writer = new StreamWriter(AppDirectory + "../2.5DHero/Security/FileValidation.vb", false, Encoding.UTF8))
            {
                foreach (char letter in string.Join("\n", Temp2))
                    Writer.Write(letter);
            }

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
