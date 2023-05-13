using System;
using PrimitiveData3D;

namespace MultilevelLibrary.Generation.Model
{
    class AreaInfo
    {
        public Vector3P StartCornerL { get; }
        public bool IsExistFutureCornerL { get; private set; }

        private Vector3P futureCornerL;
        public Vector3P FutureCornerL
        {
            get
            {
                if (!IsExistFutureCornerL)
                    throw new Exception("FutureCornerL does not exist.");
                IsExistFutureCornerL = false;
                return futureCornerL;
            }
            set
            {
                IsExistFutureCornerL = true;
                futureCornerL = value;
            }
        }


        public Vector3P StartCornerR { get; }
        public bool IsExistFutureCornerR { get; private set; }

        private Vector3P futureCornerR;
        public Vector3P FutureCornerR
        {
            get
            {
                if (!IsExistFutureCornerR)
                    throw new Exception("FutureCornerR does not exist.");
                IsExistFutureCornerR = false;
                return futureCornerR;
            }
            set
            {
                IsExistFutureCornerR = true;
                futureCornerR = value;
            }
        }

        public AreaInfo(Vector3P startCornerL, Vector3P startCornerR)
        {
            StartCornerL = startCornerL;
            StartCornerR = startCornerR;
            IsExistFutureCornerL = false;
            IsExistFutureCornerR = false;
        }
    }
}
