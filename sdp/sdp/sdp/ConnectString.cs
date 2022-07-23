using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace sdp
{
    public class ConnectString
    {
        /*  public String server = "localhost";
          public String username = "root";
          public String password = "123456";
          public String database = "sdp"; */
        private String server = "localhost";
        private String username = "root";
        private String password = "123456";
        private String database = "sdp2";
        public static String ConnectionString = "server = localhost ; user id = root ; password = 123456 ; database=sdp2";
        public ConnectString(String username, String password)
        {
            this.username = username;
            this.password = password;
        }

        public String getString()
        {
            return "server = localhost; user id =" + username + " ; password = " + password + " ; database=sdp2";
        }
    }
}
