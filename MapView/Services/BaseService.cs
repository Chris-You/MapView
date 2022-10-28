using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MapView.Services
{
    public class BaseService
    {
        public bool ExistFile(string path)
        {
            //todo 한달마다 파일을 갱신한다.

            // 캠프 리스트카 파일에 존재하는지 체크
            if (!File.Exists(path))
            {
                // 없으면 API 호출 후  json 파일 생성
                return false;
            }
            else
            {
                // 있으면 파일을 읽어들여 로드 (API 호출을 최소화)
                return true;
            }
        }


        public bool ExistDirectoryFile(string path)
        {
            if (Directory.Exists(path) == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
