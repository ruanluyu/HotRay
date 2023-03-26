using HotRay.Base.Ray;
using HotRay.Base.Ray.Lite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.SAs
{
    public class IntStringAccumulater : Accumulater<IntRay, StringRay>
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
        CodingType DecodeType { set; get; }

        public IntStringAccumulater() { DecodeType = CodingType.UTF8; }
        public IntStringAccumulater(IntStringAccumulater other):base(other) { DecodeType = other.DecodeType; }
        public override NodeBase CloneNode()
        {
            return new IntStringAccumulater(this);
        }

        public override void Init()
        {
            base.Init();
            cachedByte = null;
        }

        List<byte>? cachedByte;

        protected override async Task AccumulateBegin()
        {
            cachedByte = new List<byte>();
        }

        protected override async Task<StringRay?> AccumulatePop()
        {
            if (cachedByte != null)
            {
                Encoding enc;
                switch (DecodeType)
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
                        return null;
                }
                return await Task.Run(() => new StringRay() { Data = enc.GetString(cachedByte.ToArray()) });
            }
            return null;
        }

        protected override async Task AccumulatePush(IntRay rayT)
        {
            var d = rayT.Data;
            if(cachedByte != null) cachedByte.Add((byte)d);
        }
    }
}
