﻿using System;

/// <summary>
/// Descrizione di riepilogo per AjaxJsonResponse
/// </summary>
namespace MB.FileBrowser
{
    public class AjaxJsonResponse
    {
        public Object data { get; set; }
        public int exitcode { get; set; }
        public Boolean success { get; set; }
        public string msg;
        public string command;

        public AjaxJsonResponse()
        {
        }
    }
}