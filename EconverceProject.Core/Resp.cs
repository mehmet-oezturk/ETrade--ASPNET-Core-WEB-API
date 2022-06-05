using System.Collections.Generic;

namespace EConverceProject.Core.Controllers
{
    public class Resp<T>// Ok yi de Badrequesti de bu nesneden dönecek
    {   //dictionary iki parametre alıyordu burada ilk parametre string ikincisi ise List<string> tir

        //ihtiyaca göre CustomErrorCode
        //public int CustomErrorCode { get; set; }

        public Dictionary<string, string[]> Errors { get; private set; }//private set(dışarıdan set edilemesin)

        public T Data { get; set; }

        //listeyi hataları daha kolay eklemek için aşağıdaki islemi yapıyoruz
        public void AddError(string key, params string[] errors) //listeyi hataları daha kolay eklemek için  islemi yapıyoruz
        //AddError içinde hata alanı keyini ver sonrada params olarak (sınırsız bir şekilde) string dizisi olarak da hataları ver ve yularıdaki isteye bakıp
        {
            if (Errors == null)
            {
                Errors = new Dictionary<string, string[]>();
                //String dizisi alan bir dictionary  var ve buraya ilk önce listemizi newledik sonrada

                Errors.Add(key, errors); /*buraya virgül koyarak birden fazla ekleme yapabiliyoruz*///listemize bir item ekledik

            }
        }
    }
}
