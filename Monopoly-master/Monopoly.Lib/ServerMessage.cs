using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly.Lib
{
    public class ServerMessage
    {
        /*
         * A message is compose of 3 parts, the header that identifies what the message should do,
         * the body that represents the data being sent, and the footer that indicates the message has
         * ended.
         */
        public string messageHeader { get; set; }
        public string messageBody { get; set; }
        public const string MESSAGE_FOOTER = "MESSAGE_END";

        //Creates message from a supplied header and body
        public ServerMessage(string messageHeader, string messageBody)
        {
            this.messageHeader = messageHeader;
            this.messageBody = messageBody;
        }

        //Creates a message from a ServerMessage that has being converted to a string.
        //Used to recreate object when it's sent over the network as a string.
        [JsonConstructor]
        public ServerMessage(string serverMessageString)
        {
            string[] message = serverMessageString.Split('#');

            if (message[0] != null)
                messageHeader = message[0];
            else
                messageHeader = "Failure";

            if (message[1] != null)
                messageBody = message[1];
            else
                messageBody = "Failure";

        }

        public string GetMessageHeader()
        {
            return messageHeader;
        }

        public string GetMessageBody()
        {
            return messageBody;
        }

        public string GetMessageFooter()
        {
            return MESSAGE_FOOTER;
        }

        public string GetFullMessage()
        {
            return messageHeader + "#" + messageBody + "#" + MESSAGE_FOOTER;
        }

        public override string ToString()
        {
            return GetFullMessage();
        }
    }
}
