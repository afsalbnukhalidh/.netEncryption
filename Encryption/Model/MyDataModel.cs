﻿namespace Encryption.Model
{
    public class MyDataModel
    {
        public List<DataList> data { get; set; }
    }
    public class DataList 
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }

}
