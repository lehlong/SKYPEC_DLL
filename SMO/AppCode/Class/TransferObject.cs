using System.Web.Mvc;

namespace SMO
{
    public class TransferObject
    {
        private object _data;

        public string Type { get; set; }

        public bool State { get; set; }

        public MessageObject Message { get; set; }

        public object Data
        {
            get
            {
                return _data;
            }
            set
            {
                if (value == null)
                    return;
                _data = value;
                DataType = _data.GetType().ToString();
            }
        }

        public string DataType { get; private set; }

        public object ExtData { get; set; }

        public string ExtDataType { get; set; }

        public TransferObject()
        {
            Type = TransferType.Json;
            Message = new MessageObject();
        }

        public JsonResult ToJsonResult()
        {
            return new JsonNetResult()
            {
                Data = (this),
                ContentType = null,
                ContentEncoding = null,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public void MergeModelState(ModelStateDictionary _modelState)
        {
            if (_modelState != null && !_modelState.IsValid)
            {
                Message.Code = "0001";
                Message.Message = "Error when validate model state";
                string str = "";
                foreach (ModelState modelState in _modelState.Values)
                {
                    if (modelState.Errors.Count > 0)
                    {
                        foreach (ModelError modelError in modelState.Errors)
                            str += string.Format("{0}{1}", modelError.ErrorMessage, "<br/>");
                    }
                }
                Message.Detail = str;
            }
        }
    }

    public static class TransferType
    {
        public static readonly string Unknow = "UNKNOW";
        public static readonly string Redirect = "REDIRECT";
        public static readonly string Json = "JSON";
        public static readonly string Message = "MESSAGE";
        public static readonly string Alert = "ALERT";
        public static readonly string Dialog = "DIALOG";
        public static readonly string JsFunction = "JSFUNCTION";
        public static readonly string JsCommand = "JSCOMMAND";
        public static readonly string AlertSuccess = "ALERTSUCCESS";
        public static readonly string AlertInfo = "ALERTINFO";
        public static readonly string AlertWarning = "ALERTWARNING";
        public static readonly string AlertDanger = "ALERTDANGER";

        private static string _separator;
        private static string _multiTypePattern;

        public static string Separator
        {
            get
            {
                return TransferType._separator;
            }
            set
            {
                TransferType._separator = value;
                TransferType._multiTypePattern = "{0}" + TransferType._separator + "{1}";
            }
        }

        public static string MessageAndRedirect
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, Message, Redirect);
            }
        }

        public static string MessageAndJsFunction
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, Message, JsFunction);
            }
        }

        public static string MessageAndJsCommand
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, Message, JsCommand);
            }
        }

        public static string JsFunctionAndMessage
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, JsFunction, Message);
            }
        }

        public static string JsCommandAndMessage
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, JsCommand, Message);
            }
        }

        public static string AlertAndRedirect
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, Alert, Redirect);
            }
        }

        public static string AlertAndJsFunction
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, Alert, JsFunction);
            }
        }

        public static string AlertAndJsCommand
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, Alert, JsCommand);
            }
        }

        public static string JsFunctionAndAlert
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, JsFunction, Alert);
            }
        }

        public static string JsCommandAndAlert
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, JsCommand, Alert);
            }
        }

        public static string DialogAndRedirect
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, Dialog, Redirect);
            }
        }

        public static string DialogAndJsFunction
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, Dialog, JsFunction);
            }
        }

        public static string DialogAndJsCommand
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, Dialog, JsCommand);
            }
        }

        public static string JsFunctionAndDialog
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, JsFunction, Dialog);
            }
        }

        public static string JsCommandAndDialog
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, JsCommand, Dialog);
            }
        }

        public static string AlertSuccessAndJsFunction
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, AlertSuccess, JsFunction);
            }
        }

        public static string AlertSuccessAndJsCommand
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, AlertSuccess, JsCommand);
            }
        }

        public static string JsFunctionAndAlertSuccess
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, JsFunction, AlertSuccess);
            }
        }

        public static string JsCommandAndAlertSuccess
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, JsCommand, AlertSuccess);
            }
        }




        public static string AlertInfoAndJsFunction
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, AlertInfo, JsFunction);
            }
        }

        public static string AlertInfoAndJsCommand
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, AlertSuccess, JsCommand);
            }
        }

        public static string JsFunctionAndAlertInfo
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, JsFunction, AlertInfo);
            }
        }

        public static string JsCommandAndAlertInfo
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, JsCommand, AlertInfo);
            }
        }




        public static string AlertDangerAndJsFunction
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, AlertDanger, JsFunction);
            }
        }

        public static string AlertDangerAndJsCommand
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, AlertDanger, JsCommand);
            }
        }

        public static string JsFunctionAndAlertDanger
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, JsFunction, AlertDanger);
            }
        }

        public static string JsCommandAndAlertDanger
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, JsCommand, AlertDanger);
            }
        }








        public static string AlertWarningAndJsFunction
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, AlertWarning, JsFunction);
            }
        }

        public static string AlertWarningAndJsCommand
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, AlertWarning, JsCommand);
            }
        }

        public static string JsFunctionAndAlertWarning
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, JsFunction, AlertWarning);
            }
        }

        public static string JsCommandAndAlertWarning
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, JsCommand, AlertWarning);
            }
        }

        static TransferType()
        {
            TransferType.Separator = "_";
        }
    }
}