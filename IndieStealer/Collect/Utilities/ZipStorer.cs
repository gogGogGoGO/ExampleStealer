using System.Collections.Generic;
using System.Text;

#if NET45
using System.Threading.Tasks;
#endif

namespace System.IO.Compression
{
#if NETSTANDARD
    public static class StreamExtension
    {
        public static void Close(this Stream stream)
        {
            stream.Dispose(); 
            GC.SuppressFinalize(stream);
        }
    }
#endif

        public class ZipStorer : IDisposable
        {
            public enum Compression : ushort
            {
                Store = 0,
                Deflate = 8
            }
    
            public struct ZipFileEntry
            {
                public Compression Method;
                public string FilenameInZip;
                public uint FileSize;
                public uint CompressedSize;
                public uint HeaderOffset;
                public uint FileOffset;
                public uint HeaderSize;
                public uint Crc32;
                public DateTime ModifyTime;
                public string Comment;
                public bool EncodeUTF8;
        
                public override string ToString()
                {
                    return this.FilenameInZip;
                }
            }

#region Public fields
                public bool EncodeUTF8 = false;
                public bool ForceDeflating = false;
#endregion

#region Private fields
                private List<ZipFileEntry> Files = new List<ZipFileEntry>();
                private string FileName;
                private Stream ZipFileStream;
                private string Comment = "";
                private byte[] CentralDirImage = null;
                private ushort ExistingFiles = 0;
                private FileAccess Access;
                private bool leaveOpen;
                private static UInt32[] CrcTable = null;
                private static Encoding DefaultEncoding = Encoding.GetEncoding(437);
#endregion

#region Public methods
            static ZipStorer()
            {
                CrcTable = new UInt32[256];
                for (int i = 0; i < CrcTable.Length; i++)
                {
                    UInt32 c = (UInt32)i;
                    for (int j = 0; j < 8; j++)
                    {
                        if ((c & 1) != 0)
                            c = 3988292384 ^ (c >> 1);
                        else
                            c >>= 1;
                    }
                    CrcTable[i] = c;
                }
            }
            public static ZipStorer Create(Stream _stream, string _comment, bool _leaveOpen=false)
            {
                ZipStorer zip = new ZipStorer();
                zip.Comment = _comment;
                zip.ZipFileStream = _stream;
                zip.Access = FileAccess.Write;
                zip.leaveOpen = _leaveOpen;
                return zip;
            }
                                                                                        
                                        
                
                
            public void AddFile(Compression _method, string _pathname, string _filenameInZip, string _comment)
            {
                try
                {
                    if (Access == FileAccess.Read)
                        throw new InvalidOperationException();

                    using (var stream = new FileStream(_pathname, FileMode.Open, FileAccess.Read))
                    {
                        AddStream(_method, _filenameInZip, stream, File.GetLastWriteTime(_pathname), _comment);
                    }
                }
                catch (Exception ex)
                { }
            
            }
            public void AddStream(Compression _method, string _filenameInZip, Stream _source, DateTime _modTime, string _comment)
            {
                if (Access == FileAccess.Read)
                    throw new InvalidOperationException("Writing is not alowed");

                            ZipFileEntry zfe = new ZipFileEntry();
                zfe.Method = _method;
                zfe.EncodeUTF8 = this.EncodeUTF8;
                zfe.FilenameInZip = NormalizedFilename(_filenameInZip);
                zfe.Comment = _comment ?? "";

                zfe.Crc32 = 0;              
                zfe.HeaderOffset = (uint)this.ZipFileStream.Position;              
                zfe.ModifyTime = _modTime;

                            WriteLocalHeader(ref zfe);
                zfe.FileOffset = (uint)this.ZipFileStream.Position;

                            Store(ref zfe, _source);
                _source.Close();

                this.UpdateCrcAndSizes(ref zfe);

                Files.Add(zfe);
            }
        
            public void AddDirectory(Compression _method, string _pathname, string _pathnameInZip, string _comment = null)
            {
                if (Access == FileAccess.Read)
                    throw new InvalidOperationException("Writing is not allowed");

                string foldername;
                int pos = _pathname.LastIndexOf(Path.DirectorySeparatorChar);
                string separator = Path.DirectorySeparatorChar.ToString();

                if (pos >= 0)
                    foldername = _pathname.Remove(0, pos + 1);
                else
                    foldername = _pathname;

                if (!string.IsNullOrEmpty(_pathnameInZip))
                    foldername = _pathnameInZip + foldername;

                if (!foldername.EndsWith(separator, StringComparison.CurrentCulture))
                    foldername = foldername + separator;

            
                            string[] fileEntries = Directory.GetFiles(_pathname);

                foreach (string fileName in fileEntries)
                    this.AddFile(_method, fileName, foldername + Path.GetFileName(fileName), "");

                            string[] subdirectoryEntries = Directory.GetDirectories(_pathname);

                foreach (string subdirectory in subdirectoryEntries)
                    this.AddDirectory(_method, subdirectory, foldername, "");
            }
                                                    public void Close()
            {
                try
                {
                    if (this.Access != FileAccess.Read)
                    {
                        uint centralOffset = (uint)this.ZipFileStream.Position;
                        uint centralSize = 0;

                        if (this.CentralDirImage != null)
                            this.ZipFileStream.Write(CentralDirImage, 0, CentralDirImage.Length);

                        for (int i = 0; i < Files.Count; i++)
                        {
                            long pos = this.ZipFileStream.Position;
                            this.WriteCentralDirRecord(Files[i]);
                            centralSize += (uint)(this.ZipFileStream.Position - pos);
                        }

                        if (this.CentralDirImage != null)
                            this.WriteEndRecord(centralSize + (uint)CentralDirImage.Length, centralOffset);
                        else
                            this.WriteEndRecord(centralSize, centralOffset);
                    }

                    if (this.ZipFileStream != null && !this.leaveOpen)
                    {
                        this.ZipFileStream.Flush();
                        this.ZipFileStream.Dispose();
                        this.ZipFileStream = null;
                    }
                } catch { }
            }
        
               
#if NET45
                                                                
        public async Task<bool> ExtractFileAsync(ZipFileEntry _zfe, Stream _stream)
        {
            if (!_stream.CanWrite)
                throw new InvalidOperationException("Stream cannot be written");

                        byte[] signature = new byte[4];
            this.ZipFileStream.Seek(_zfe.HeaderOffset, SeekOrigin.Begin);
            await this.ZipFileStream.ReadAsync(signature, 0, 4).ConfigureAwait(false);
            if (BitConverter.ToUInt32(signature, 0) != 0x04034b50)
                return false;

                        Stream inStream;
            if (_zfe.Method == Compression.Store)
                inStream = this.ZipFileStream;
            else if (_zfe.Method == Compression.Deflate)
                inStream = new DeflateStream(this.ZipFileStream, CompressionMode.Decompress, true);
            else
                return false;

                        byte[] buffer = new byte[16384];
            this.ZipFileStream.Seek(_zfe.FileOffset, SeekOrigin.Begin);
            uint bytesPending = _zfe.FileSize;
            while (bytesPending > 0)
            {
                int bytesRead = await inStream.ReadAsync(buffer, 0, (int)Math.Min(bytesPending, buffer.Length)).ConfigureAwait(false);
                await _stream.WriteAsync(buffer, 0, bytesRead).ConfigureAwait(false);
                bytesPending -= (uint)bytesRead;
            }
            _stream.Flush();

            if (_zfe.Method == Compression.Deflate)
                inStream.Dispose();
            return true;
        }
#endif              
#endregion

#region Private methods
                                
                                        
                        /* Local file header:
            local file header signature     4 bytes  (0x04034b50)
            version needed to extract       2 bytes
            general purpose bit flag        2 bytes
            compression method              2 bytes
            last mod file time              2 bytes
            last mod file date              2 bytes
            crc-32                          4 bytes
            compressed size                 4 bytes
            uncompressed size               4 bytes
            filename length                 2 bytes
            extra field length              2 bytes
            filename (variable size)
            extra field (variable size)
        */
        private void WriteLocalHeader(ref ZipFileEntry _zfe)
        {
            long pos = this.ZipFileStream.Position;
            Encoding encoder = _zfe.EncodeUTF8 ? Encoding.UTF8 : DefaultEncoding;
            byte[] encodedFilename = encoder.GetBytes(_zfe.FilenameInZip);

            this.ZipFileStream.Write(new byte[] { 80, 75, 3, 4, 20, 0}, 0, 6);             
            this.ZipFileStream.Write(BitConverter.GetBytes((ushort)(_zfe.EncodeUTF8 ? 0x0800 : 0)), 0, 2);             this.ZipFileStream.Write(BitConverter.GetBytes((ushort)_zfe.Method), 0, 2);              this.ZipFileStream.Write(BitConverter.GetBytes(DateTimeToDosTime(_zfe.ModifyTime)), 0, 4);             this.ZipFileStream.Write(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 0, 12);             this.ZipFileStream.Write(BitConverter.GetBytes((ushort)encodedFilename.Length), 0, 2);             this.ZipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2); 
            this.ZipFileStream.Write(encodedFilename, 0, encodedFilename.Length);
            _zfe.HeaderSize = (uint)(this.ZipFileStream.Position - pos);
        }
        /* Central directory's File header:
            central file header signature   4 bytes  (0x02014b50)
            version made by                 2 bytes
            version needed to extract       2 bytes
            general purpose bit flag        2 bytes
            compression method              2 bytes
            last mod file time              2 bytes
            last mod file date              2 bytes
            crc-32                          4 bytes
            compressed size                 4 bytes
            uncompressed size               4 bytes
            filename length                 2 bytes
            extra field length              2 bytes
            file comment length             2 bytes
            disk number start               2 bytes
            internal file attributes        2 bytes
            external file attributes        4 bytes
            relative offset of local header 4 bytes
            filename (variable size)
            extra field (variable size)
            file comment (variable size)
        */
        private void WriteCentralDirRecord(ZipFileEntry _zfe)
        {
            Encoding encoder = _zfe.EncodeUTF8 ? Encoding.UTF8 : DefaultEncoding;
            byte[] encodedFilename = encoder.GetBytes(_zfe.FilenameInZip);
            byte[] encodedComment = encoder.GetBytes(_zfe.Comment);

            this.ZipFileStream.Write(new byte[] { 80, 75, 1, 2, 23, 0xB, 20, 0 }, 0, 8);
            this.ZipFileStream.Write(BitConverter.GetBytes((ushort)(_zfe.EncodeUTF8 ? 0x0800 : 0)), 0, 2);             
            this.ZipFileStream.Write(BitConverter.GetBytes((ushort)_zfe.Method), 0, 2);              
            this.ZipFileStream.Write(BitConverter.GetBytes(DateTimeToDosTime(_zfe.ModifyTime)), 0, 4);              
            this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.Crc32), 0, 4);             
            this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.CompressedSize), 0, 4);             
            this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.FileSize), 0, 4);             
            this.ZipFileStream.Write(BitConverter.GetBytes((ushort)encodedFilename.Length), 0, 2);             
            this.ZipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2);             
            this.ZipFileStream.Write(BitConverter.GetBytes((ushort)encodedComment.Length), 0, 2);

            this.ZipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2);             
            this.ZipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2);             
            this.ZipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2);             
            this.ZipFileStream.Write(BitConverter.GetBytes((ushort)0x8100), 0, 2);            
            this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.HeaderOffset), 0, 4);  
            this.ZipFileStream.Write(encodedFilename, 0, encodedFilename.Length);
            this.ZipFileStream.Write(encodedComment, 0, encodedComment.Length);
        }
        /* End of central dir record:
            end of central dir signature    4 bytes  (0x06054b50)
            number of this disk             2 bytes
            number of the disk with the
            start of the central directory  2 bytes
            total number of entries in
            the central dir on this disk    2 bytes
            total number of entries in
            the central dir                 2 bytes
            size of the central directory   4 bytes
            offset of start of central
            directory with respect to
            the starting disk number        4 bytes
            zipfile comment length          2 bytes
            zipfile comment (variable size)
        */
        private void WriteEndRecord(uint _size, uint _offset)
        {
            Encoding encoder = this.EncodeUTF8 ? Encoding.UTF8 : DefaultEncoding;
            byte[] encodedComment = encoder.GetBytes(this.Comment);

            this.ZipFileStream.Write(new byte[] { 80, 75, 5, 6, 0, 0, 0, 0 }, 0, 8);
            this.ZipFileStream.Write(BitConverter.GetBytes((ushort)Files.Count+ExistingFiles), 0, 2);
            this.ZipFileStream.Write(BitConverter.GetBytes((ushort)Files.Count+ExistingFiles), 0, 2);
            this.ZipFileStream.Write(BitConverter.GetBytes(_size), 0, 4);
            this.ZipFileStream.Write(BitConverter.GetBytes(_offset), 0, 4);
            this.ZipFileStream.Write(BitConverter.GetBytes((ushort)encodedComment.Length), 0, 2);
            this.ZipFileStream.Write(encodedComment, 0, encodedComment.Length);
        }
        private void Store(ref ZipFileEntry _zfe, Stream _source)
        {
            byte[] buffer = new byte[16384];
            int bytesRead;
            uint totalRead = 0;
            Stream outStream;

            long posStart = this.ZipFileStream.Position;
            long sourceStart = _source.CanSeek ? _source.Position : 0;

            if (_zfe.Method == Compression.Store)
                outStream = this.ZipFileStream;
            else
                outStream = new DeflateStream(this.ZipFileStream, CompressionMode.Compress, true);

            _zfe.Crc32 = 0 ^ 0xffffffff;
            
            do
            {
                bytesRead = _source.Read(buffer, 0, buffer.Length);
                totalRead += (uint)bytesRead;
                if (bytesRead > 0)
                {
                    outStream.Write(buffer, 0, bytesRead);

                    for (uint i = 0; i < bytesRead; i++)
                    {
                        _zfe.Crc32 = ZipStorer.CrcTable[(_zfe.Crc32 ^ buffer[i]) & 0xFF] ^ (_zfe.Crc32 >> 8);
                    }
                }
            } while (bytesRead > 0);
            outStream.Flush();

            if (_zfe.Method == Compression.Deflate)
                outStream.Dispose();

            _zfe.Crc32 ^= 0xffffffff;
            _zfe.FileSize = totalRead;
            _zfe.CompressedSize = (uint)(this.ZipFileStream.Position - posStart);

            if (_zfe.Method == Compression.Deflate && !this.ForceDeflating && _source.CanSeek && _zfe.CompressedSize > _zfe.FileSize)
            {
                _zfe.Method = Compression.Store;
                this.ZipFileStream.Position = posStart;
                this.ZipFileStream.SetLength(posStart);
                _source.Position = sourceStart;
                this.Store(ref _zfe, _source);
            }
        }
        /* DOS Date and time:
            MS-DOS date. The date is a packed value with the following format. Bits Description 
                0-4 Day of the month (131) 
                5-8 Month (1 = January, 2 = February, and so on) 
                9-15 Year offset from 1980 (add 1980 to get actual year) 
            MS-DOS time. The time is a packed value with the following format. Bits Description 
                0-4 Second divided by 2 
                5-10 Minute (059) 
                11-15 Hour (023 on a 24-hour clock) 
        */
        private uint DateTimeToDosTime(DateTime _dt)
        {
            return (uint)(
                (_dt.Second / 2) | (_dt.Minute << 5) | (_dt.Hour << 11) | 
                (_dt.Day<<16) | (_dt.Month << 21) | ((_dt.Year - 1980) << 25));
        }
                                                                
                
                
        /* CRC32 algorithm
          The 'magic number' for the CRC is 0xdebb20e3.  
          The proper CRC pre and post conditioning is used, meaning that the CRC register is
          pre-conditioned with all ones (a starting value of 0xffffffff) and the value is post-conditioned by
          taking the one's complement of the CRC residual.
          If bit 3 of the general purpose flag is set, this field is set to zero in the local header and the correct
          value is put in the data descriptor and in the central directory.
        */
        private void UpdateCrcAndSizes(ref ZipFileEntry _zfe)
        {
            long lastPos = this.ZipFileStream.Position;  
            this.ZipFileStream.Position = _zfe.HeaderOffset + 8;
            this.ZipFileStream.Write(BitConverter.GetBytes((ushort)_zfe.Method), 0, 2);  
            this.ZipFileStream.Position = _zfe.HeaderOffset + 14;
            this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.Crc32), 0, 4);              
            this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.CompressedSize), 0, 4);              
            this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.FileSize), 0, 4);  
            this.ZipFileStream.Position = lastPos;          
        }
        private string NormalizedFilename(string _filename)
        {
            string filename = _filename.Replace('\\', '/');

            int pos = filename.IndexOf(':');
            if (pos >= 0)
                filename = filename.Remove(0, pos + 1);

            return filename.Trim('/');
        }
                                        
                                                                                        
                                
                        
                                        
                                                        
        #endregion

#region IDisposable Members
        public void Dispose()
        {
            this.Close();
        }
#endregion
        }
}