﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray.Hot
{

    public class TensorRay<numberT> : MemoryRay<numberT>
        where numberT:struct
    {
        static readonly numberT[] sharedEmptyData = new numberT[0];
        static readonly int[] sharedEmptyDims = new int[0];

        public int[] Dims { get; private set; }

        public TensorRay():base()
        {
            Data = sharedEmptyData;
            Dims = sharedEmptyDims;
        }

        public TensorRay(TensorRay<numberT> other):base(other)
        {
            Dims = (other.Dims == sharedEmptyDims) ? sharedEmptyDims : other.Dims!.ToArray();
        }

        protected virtual void _Reshape(params int[] dims)
        {
            Dims = dims;
            if (Dims.Length == 0)
            {
                Data = sharedEmptyData;
                Dims = sharedEmptyDims;
            }
            else
            {
                long totalLength = 1;
                checked
                {
                    for (int i = 0; i < Dims.Length; i++)
                    {
                        totalLength = totalLength * Dims[i];
                    }
                }
                if (totalLength > 0) ResizeMemory(totalLength);
                else Data = sharedEmptyData;
            }
            
        }

        public override RayBase RayClone()
        {
            return new TensorRay<numberT>(this);
        }

        protected virtual long _CoordToID(params int[] coords)
        {
            // if (Dims.Length == 0) throw new IndexOutOfRangeException();
            // if (coords.Length == 0) throw new ArgumentNullException();
            long rid = coords[0];
            long mul = Dims[0];
            for (int i = 1; i < Dims.Length; i++)
            {
                if (i >= coords.Length) break;
                if (coords[i] >= Dims[i]) throw new IndexOutOfRangeException();
                rid += mul * coords[i];
                if (i < Dims.Length - 1) 
                    mul *= Dims[i];
            }
            return rid;
        }

        protected virtual void _IDToCoord(int id, int[] coords)
        {
            // if (Dims.Length != coords.Length) throw new ArgumentException("coords");
            for (int i = 0; i < Dims.Length; i++)
            {
                coords[i] = id % Dims[i];
                id /= Dims[i];
            }
        }

        protected virtual numberT _Get(params int[] ids)
        {
            try
            {
                return Data![_CoordToID(ids)];
            }
            catch (Exception)
            {
            }
            return default;
        }

        protected virtual void _Set(numberT n, params int[] ids)
        {
            try
            {
                Data![_CoordToID(ids)] = n;
            }
            catch (Exception)
            {
            }
        }

        protected virtual void _Fill(numberT n)
        {
            if (Empty) return;
            for (int i = 0; i < Data!.Length; i++)
            {
                Data[i] = n;
            }
        }


        protected virtual TensorRay<numberT> _Slice(int[] start, int[] count)
        {
            var res = new TensorRay<numberT>();
            if (!Empty)
            {
                if (start.Length != count.Length) throw new ArgumentException("start.Length != count.Length");
                if (start.Length != Dims.Length) throw new ArgumentException("start.Length != Dims.Length");
                int[] cache = new int[start.Length];
                for (int i = 0; i < start.Length; i++)
                {
                    if (start[i] >= Dims[i]) throw new IndexOutOfRangeException("start");
                    cache[i] = Math.Min(Dims[i] - start[i], count[i]);
                }
                res._Reshape(cache);
                if (!res.Empty)
                {
                    for (int i = 0; i < res.Data!.Length; i++)
                    {
                        res._IDToCoord(i, cache);
                        for (int j = 0; j < cache.Length; j++)
                            cache[j] += start[j];
                        res.Data[i] = _Get(cache);
                    }
                }
            }
            return res;
        }
    }
}
