using System;
using System.Diagnostics;
using System.Text;
using System.Security.Cryptography;
using System.Net.Http;

string token = "{token}";
string secret = "{secret}";
DateTime dt1970 = new DateTime(1970, 1, 1);
DateTime current = DateTime.Now;
TimeSpan span = current - dt1970;
long time = Convert.ToInt64(span.TotalMilliseconds);
string nonce = Guid.NewGuid().ToString();
string data = token + time.ToString() + nonce;
Encoding utf8 = Encoding.UTF8;
HMACSHA256 hmac = new HMACSHA256(utf8.GetBytes(secret));
string signature = Convert.ToBase64String(hmac.ComputeHash(utf8.GetBytes(data)));

Console.WriteLine("Authorization: "+token);
Console.WriteLine("sign: "+ signature);
Console.WriteLine("nonce: " + nonce);
Console.WriteLine("t: " + time.ToString());
