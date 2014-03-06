 #region License
  /*
  The MIT License (MIT)
 
  Copyright (c) 2013-2014 The SharpFlame Authors.
 
  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files (the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:
 
  The above copyright notice and this permission notice shall be included in
  all copies or substantial portions of the Software.
 
  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
  THE SOFTWARE.
  */
 #endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Eto.Forms;
using Ninject.Extensions.Logging;

namespace SharpFlame.Old.Settings
{
    public sealed class InvalidKeyException : Exception
    {
        public InvalidKeyException()
        {
        }

        public InvalidKeyException(string message) 
            : base(message)
        {
        }

        public InvalidKeyException(string message,
            Exception innerException): base(message, innerException)
        {
        }
    }

    public class KeyboardKey {
        private Keys? key;
        public Keys? Key { 
            get { return key; } 
            private set { 
                key = value; 
                IsChar = false;
            }
        }

        private char? keyChar;
        public char? KeyChar { 
            get { return keyChar; }
            private set { 
                keyChar = value;
                IsChar = true;
            }
        }

        public bool IsChar { get; private set; }
        public bool Invalid { get; set; }

        public KeyboardKey(Keys? key = null, char? keyChar = null, bool repeat = false) 
        {
            if(key != null)
            {
                Key = key;
            } else if(keyChar != null)
            {
                KeyChar = keyChar;
            } else
            {
                throw new InvalidKeyException("Give either a key or a keyChar.");
            }
            Invalid = false;
        }

        public new string ToString() 
        {
            string text = Invalid ? "!! " : "";
            if (IsChar) {
                return text + ((char)KeyChar).ToString();
            } else {
                return text + ((Keys)Key).ToShortcutString();
            }
        }
    }

    public class KeyboardManager
    {
        private readonly ILogger logger;

        public readonly Dictionary<string, KeyboardKey> Keys;
        readonly Dictionary<Keys, KeyboardKey> keyLookupTable;
        readonly Dictionary<char, KeyboardKey> charLookupTable;

        public KeyboardManager(ILoggerFactory logFactory)
        {
            logger = logFactory.GetCurrentClassLogger();

            Keys = new Dictionary<string, KeyboardKey> ();
            keyLookupTable = new Dictionary<Keys, KeyboardKey> ();
            charLookupTable = new Dictionary<char, KeyboardKey>();
        }

        public bool Create(string name, Keys? key = null, char? keyChar = null, bool repeat = false) 
        {
            if (Keys.ContainsKey(name)) {
                throw new Exception(string.Format("The key \"{0}\" does exist.", name));
            }

            KeyboardKey kkey;
            if(key == null && keyChar == null)
            {
                kkey = new KeyboardKey(Eto.Forms.Keys.None, null);
                kkey.Invalid = true;
                Keys.Add(name, kkey);
                return false;
            }

            kkey = new KeyboardKey (key, keyChar, repeat);
            Keys.Add (name, kkey);
            if(kkey.IsChar)
            {
                try
                {
                    charLookupTable.Add((char)keyChar, kkey);
                } catch(System.ArgumentException)
                {
                    kkey.Invalid = true;
                    logger.Error("Tried to add key \"{0}\", keyChar: \"{1}\" but it already exists.", name, keyChar);
                    return false;
                }
            } else
            {
                try
                {
                    keyLookupTable.Add((Keys)key, kkey);
                } catch(System.ArgumentException)
                {
                    kkey.Invalid = true;
                    logger.Error("Tried to add key \"{0}\", key: \"{1}\" but it already exists.", name, ((Keys)key).ToShortcutString());
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="key">Key.</param>
        public void Update(string name, Keys? key = null, char? keyChar = null, bool repeat = false)
        {
            if (!Keys.ContainsKey(name)) {
                throw new Exception(string.Format("The key \"{0}\" does not exist.", name));
            }

            var kkey = Keys [name];
            Keys.Remove (name);
            if(kkey.IsChar)
            {
                charLookupTable.Remove((char)kkey.KeyChar);
            } else
            {
                keyLookupTable.Remove((Keys)kkey.Key);
            }

            Create(name, key, keyChar, repeat);
        }

        public void Update(string name, KeyboardKey kkey) {
            Update(name, kkey.Key, kkey.KeyChar);
        }

        public void HandleKeyUp(object sender, KeyEventArgs e)
        {
            Console.WriteLine ("UP Key: {0}, Char: {1}, Handled: {2}", e.KeyData, e.IsChar ? e.KeyChar.ToString() : "no char", e.Handled);
            e.Handled = true;
        }

        public void HandleKeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine ("DOWN Key: {0}, Char: {1}, Handled: {2}", e.KeyData, e.IsChar ? e.KeyChar.ToString() : "no char", e.Handled);
            e.Handled = true;
        }

    }
}
