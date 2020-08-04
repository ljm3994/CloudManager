using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
namespace googlecloud1.login
{
    public class FileDataStore
    {
        readonly string folderPath;
        private const string Key = "abcd$efgallc^loude123#vg54hy9*02w%e5";
        /// <summary>폴더의 경로.</summary>
        public string FolderPath { get { return folderPath; } }

        /// <summary>
        /// file data store의 생성자 (저장될 폴더의 경로 설정)
        /// </summary>
        /// <param name="folder">저장될 폴더의 경로</param>
        /// <param name="fullPath">
        /// 절대적인 혹은 상대적인 전체 경로 (기본값 false)
        /// <see cref="Environment.SpecialFolder.ApplicationData"/>.
        /// </param>
        public FileDataStore(string folder, bool fullPath = false)
        {
            folderPath = fullPath
                ? folder
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), folder);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        /// <summary>
        /// 얻어온 키와 값으로 새로운 파일을 만들어서 저장한다<see cref="GenerateStoredKey"/>) in 
        /// <see cref="FolderPath"/>.
        /// </summary>
        /// <typeparam name="T">저장될 데이터의 타입형식.</typeparam>
        /// <param name="key">key값 저장 데이터의 분류를 위한 key</param>
        /// <param name="value">저장될 데이터의 내부 값 (ex:토큰이나 코드 같은 종류의 값들).</param>
        public Task StoreAsync<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }
            JavaScriptSerializer json = new JavaScriptSerializer();
            System.Text.StringBuilder bulider = new System.Text.StringBuilder();
            json.Serialize(value, bulider);
            string serialized = bulider.ToString();
            var filePath = Path.Combine(folderPath, GenerateStoredKey(key, typeof(T)));
            File.WriteAllText(filePath, EncryptString(serialized));
            return TaskEx.Delay(0);
        }
        /// <summary>
        ///  키값을 이용하여 정해진 파일안에 데이터를 삭제한다.
        /// <see cref="FolderPath"/>.
        /// </summary>
        /// <param name="key">삭제할 데이터의 key값</param>
        public Task DeleteAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }

            var filePath = Path.Combine(folderPath, GenerateStoredKey(key, typeof(T)));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return TaskEx.Delay(0);
        }

        /// <summary>
        /// 매개 변수로 넘어온 키값을 이용하여 파일안의 데이터를 가져온다
        /// </summary>
        /// <typeparam name="T">가져올 데이터의 타입형식</typeparam>
        /// <param name="key">가져올 데이터의 key값</param>
        /// <returns>저장된 데이터를 반환</returns>
        public Task<T> GetAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }

            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            var filePath = Path.Combine(folderPath, GenerateStoredKey(key, typeof(T)));
            if (File.Exists(filePath))
            {
                try
                {
                    var obj = File.ReadAllText(filePath);
                    string text = DecryptString(obj);
                    JavaScriptSerializer json = new JavaScriptSerializer();
                    tcs.SetResult(json.Deserialize<T>(text));
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }
            else
            {
                tcs.SetResult(default(T));
            }
            return tcs.Task;
        }
        //public Task<List<T>> AllGetAsync<T>()
        //{
        //}
        /// <summary>
        /// 모든 파일안의 데이터를 전부 지운다
        /// </summary>
        public Task ClearAsync()
        {
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
                Directory.CreateDirectory(folderPath);
            }

            return TaskEx.Delay(0);
        }
        public Task<List<T>> AllLoad<T>()
        {
            TaskCompletionSource<List<T>> tcs = new TaskCompletionSource<List<T>>();
            if (Directory.Exists(folderPath))
            {
                try
                {
                    List<T> list = new List<T>();
                    DirectoryInfo di = new DirectoryInfo(folderPath);
                    FileInfo[] file = di.GetFiles();
                    foreach (var item in file)
                    {
                        var obj = File.ReadAllText(item.FullName);
                        string text = DecryptString(obj);
                        JavaScriptSerializer json = new JavaScriptSerializer();
                        list.Add(json.Deserialize<T>(text));
                    }
                    tcs.SetResult(list);
                }
                catch
                { 
                }
            }
            else
            { 
            }
            return tcs.Task;
        }
        /// <summary>클래스 타입과 넘어온 key값을 이용하여 중복되지 않은 유니크한 key를 만든다</summary>
        /// <param name="key">데이터의 key값.</param>
        /// <param name="t">저장 타입</param>
        public static string GenerateStoredKey(string key, Type t)
        {
            return string.Format("{0}-{1}.logininfo", t.Name, key);
        }
        /// <summary>
        /// AES256방식을 사용하여 암호화 한다.
        /// </summary>
        /// <param name="InputText">암호화 하려는 문자열</param>
        /// <returns></returns>
        private string EncryptString(string InputText)
        {
            // AesManaged 클래스 선언
            RijndaelManaged aes = new RijndaelManaged();
            //매개변수로 넘어온 문자열을 바이트 배열로 변환
            byte[] PText = System.Text.Encoding.Unicode.GetBytes(InputText);
            //딕셔너리 공격을 대비하여 키를 더 풀기 어렵게 만들기 위해 byte배열을 만든다
            byte[] Hard = System.Text.Encoding.ASCII.GetBytes(Key.Length.ToString());
            // PasswordDeriveBytes 클래스를 선언한다.
            PasswordDeriveBytes Secure = new PasswordDeriveBytes(Key, Hard);
            aes.KeySize = 256;
            aes.BlockSize = 128;
            //CBC 모드를 사용한다.
            aes.Mode = CipherMode.CBC;
            // SecretKey는 위에서 설정한 사이즈인 32바이트를 사용한다. (256bit가 32바이트 이다)
            aes.Key = Secure.GetBytes(32);
            // 초기화 IV백터값을 위에서 설정한 블록사이즈 만큼 준다. (128bit가 16바이트 이다)
            aes.IV = Secure.GetBytes(16);
            aes.Padding = PaddingMode.PKCS7;
            ICryptoTransform Encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] Buf = null;
            // 메모리 스트림 클래스를 선언
            using(MemoryStream mstream = new MemoryStream())
            {
                // CryptoStream 클래스를 쓰기모드로 클래스 생성한다.
                using(CryptoStream crtpto = new CryptoStream(mstream, Encrypt, CryptoStreamMode.Write))
                {
                    //암호화 프로세스가 진행되면서 memortstream에 암호화된 내용을 쓴다.
                    crtpto.Write(PText, 0, PText.Length);
                    // 암호화 종료
                    crtpto.FlushFinalBlock();
                    // 스트림 해제
                    crtpto.Close();
                }
                // 메모리 스트림에 써진 내용을 바이트 배열로 바꾼다.
                Buf = mstream.ToArray();
                // 스트림 해제
                mstream.Close();
            }
            //암호화된 내용을 ToBase64 인코딩된 문자열로 바꾼다.
            string date = Convert.ToBase64String(Buf);
            // 완성된 문자열을 반환해준다.
            return date;
        }
        /// <summary>
        /// AES256방식을 사용하는 복호화 메서드
        /// </summary>
        /// <param name="InputText">복호화할 문자열</param>
        /// <returns></returns>
        private string DecryptString(string InputText)
        {
            RijndaelManaged aes = new RijndaelManaged();

            byte[] PText = Convert.FromBase64String(InputText);
            byte[] Hard = System.Text.Encoding.ASCII.GetBytes(Key.Length.ToString());

            PasswordDeriveBytes Secure = new PasswordDeriveBytes(Key, Hard);
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Secure.GetBytes(32);
            aes.IV = Secure.GetBytes(16);
            ICryptoTransform Decrypt = aes.CreateDecryptor();
            // 여기까지는 위에 암호화 방식이랑 똑같은 방식을 사용하여 암호화와 복호화 간의 형식을 맞춰준다.
            // 복호화된 바이트를 담을 바이트 배열 선언
            byte[] Buf = null;
            int decrpyt = 0;
            using(MemoryStream memorystream = new MemoryStream(PText))
            {
                using(CryptoStream crypto = new CryptoStream(memorystream, Decrypt, CryptoStreamMode.Read))
                {
                    // 길이는 복호화 되기 전의 길이와 같기에 그 길이만큼 배열 선언을 해준다.(여기서는 총 길이는 알수 없지만 적어도 복호화 되기 전의 길이보다 커질일은 없기 때문에 이렇게 지정해놔도 충분하다)
                    Buf = new byte[PText.Length];
                    // 바이트 배열의 수를 반환해준다.
                    decrpyt = crypto.Read(Buf, 0, Buf.Length);
                    crypto.Close();
                }
                memorystream.Close();
            }
            // 복호화된 데이터를 위에 암호화의 ENCODING방식과 똑같이 문자열로 ENCODING해준다.
            string Text = System.Text.Encoding.Unicode.GetString(Buf, 0, decrpyt);
            return Text;
        }       
    }  
}
