using HotRay.Base.Ray;
using HotRay.Base.Ray.Lite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.SAs
{
    public class StringIntSequencer:Sequencer<StringRay, IntRay>
    {
        public enum CodingType
        {
            ASCII,
            UTF8,
            UTF16LE,
            UTF16BE,
            UTF32LE,
            UTF32BE,
        }

        public CodingType EncodeType
        {
            get;
            set;
        }


        public StringIntSequencer() 
        { 
            EncodeType = CodingType.UTF8; 
        }
        public StringIntSequencer(StringIntSequencer other):base(other) 
        {
            EncodeType = other.EncodeType;
        }

        public override NodeBase CloneNode()
        {
            return new StringIntSequencer(this);
        }

        protected override async IAsyncEnumerator<IntRay?> ApplySplit(StringRay? inRay)
        {
            if (inRay != null)
            {
                var s = inRay.Data;
                if(s != null)
                {
                    Encoding enc;
                    switch (EncodeType)
                    {
                        case CodingType.ASCII:
                            enc = Encoding.ASCII;
                            break;
                        case CodingType.UTF8:
                            enc = Encoding.UTF8;
                            break;
                        case CodingType.UTF16LE:
                            enc = Encoding.Unicode;
                            break;
                        case CodingType.UTF16BE:
                            enc = Encoding.BigEndianUnicode;
                            break;
                        case CodingType.UTF32LE:
                            enc = Encoding.UTF32;
                            break;
                        case CodingType.UTF32BE:
                            enc = new UTF32Encoding(true /*bigEndian*/, true /*byteOrderMark*/);
                            break;
                        default:
                            yield break;
                    }
                    foreach (var b in await Task.Run(() => enc.GetBytes(s)))
                    {
                        yield return new IntRay() { Data = b };
                    }
                }
            }
        }
    }
}
