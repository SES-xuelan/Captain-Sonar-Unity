using System;
using SimpleJson;

namespace Pomelo.DotNetClient
{
	public class Message
	{
		public MessageType type;
		public string route = null;
		public uint id;
		public JsonObject jsonObj = null;
		public string rawString = null;

		public Message (MessageType type, string route, JsonObject data)
		{
			this.type = type;
			this.route = route;
			this.jsonObj = data;
		}

		public Message (MessageType type, uint id)
		{
			this.type = type;
			this.id = id;
			this.route = "";
			this.jsonObj = null;
		}

		public Message (MessageType type, uint id, string route, JsonObject data, string rawString)
		{
			this.type = type;
			this.id = id;
			this.route = route;
			this.jsonObj = data;
			this.rawString = rawString;
		}

		public override string ToString ()
		{
			
			string str = "";
			str = str + "route:" + this.route + "|";
			str = str + "type:" + this.type.ToString () + "|";
			if (this.id != 0)
				str = str + "id:" + this.id.ToString () + "|";
			if (this.rawString != null)
				str = str + "rawString:" + this.rawString + "|";
			str = str + "jsonObj:" + this.jsonObj.ToString ();
			
			return str;
		}
	}
}