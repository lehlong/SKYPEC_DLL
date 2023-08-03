using SMO.Core.Entities;
using SMO.Repository.Implement.AD;
using SMO.Repository.Implement.MD;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Service.AD
{
    public class FormService : GenericService<T_AD_FORM, FormRepo>
    {
        public T_AD_FORM_OBJECT ObjObject { get; set; }
        public List<T_AD_FORM_OBJECT> ObjListObject { get; set; }
        public bool IsCopy { get; set; }
        public string FormCopy { get; set; }
        public FormService() : base()
        {
            ObjObject = new T_AD_FORM_OBJECT();
            ObjListObject = new List<T_AD_FORM_OBJECT>();
            IsCopy = false;
        }

        public void SearchObject()
        {
            try
            {
                ObjListObject = UnitOfWork.Repository<FormObjectRepo>().Search(ObjObject, NumerRecordPerPage, Page, out int iTotalRecord).ToList();
                TotalRecord = iTotalRecord;
            }
            catch (Exception ex)
            {
                State = false;
                Exception = ex;
            }
        }

        public void CreateObject()
        {
            try
            {
                if (!UnitOfWork.Repository<FormObjectRepo>().CheckExist(x => x.OBJECT_CODE == ObjObject.OBJECT_CODE && x.FK_FORM == ObjObject.FK_FORM))
                {
                    UnitOfWork.BeginTransaction();
                    ObjObject.PKID = Guid.NewGuid().ToString();
                    UnitOfWork.Repository<FormObjectRepo>().Create(ObjObject);

                    var lstLang = UnitOfWork.Repository<DictionaryRepo>().Queryable().Where(x => x.FK_DOMAIN == "LANG" && x.LANG == "vi").ToList();
                    foreach (var item in lstLang)
                    {
                        var obj = new T_AD_LANGUAGE()
                        {
                            PKID = Guid.NewGuid().ToString(),
                            FK_CODE = ObjObject.OBJECT_CODE,
                            OBJECT_TYPE = ObjObject.TYPE,
                            LANG = item.CODE,
                            FORM_CODE = ObjObject.FK_FORM,
                            VALUE = ObjObject.OBJECT_CODE
                        };
                        UnitOfWork.Repository<LanguageRepo>().Create(obj);
                    }
                    UnitOfWork.Commit();
                }
                else
                {
                    UnitOfWork.Rollback();
                    State = false;
                    MesseageCode = "1101";
                }
            }
            catch (Exception ex)
            {
                State = false;
                Exception = ex;
            }
        }

        public void UpdateObject()
        {
            try
            {
                UnitOfWork.BeginTransaction();
                UnitOfWork.Repository<FormObjectRepo>().Update(ObjObject);
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }

        public override void Create()
        {
            try
            {
                var lstNewLang = new List<T_AD_LANGUAGE>();
                if (!CheckExist(x => x.CODE == ObjDetail.CODE))
                {
                    UnitOfWork.BeginTransaction();
                    if (ProfileUtilities.User != null)
                    {
                        ObjDetail.CREATE_BY = ProfileUtilities.User.USER_NAME;
                        ObjDetail.CREATE_DATE = CurrentRepository.GetDateDatabase();
                    }

                    ObjDetail = CurrentRepository.Create(ObjDetail);
                    if (IsCopy)
                    {
                        //Copy object of form
                        var lstObject = UnitOfWork.Repository<FormObjectRepo>().Queryable().Where(x => x.FK_FORM == FormCopy);
                        foreach (var item in lstObject)
                        {
                            UnitOfWork.Repository<FormObjectRepo>().Detach(item);
                            item.PKID = Guid.NewGuid().ToString();
                            item.FK_FORM = ObjDetail.CODE;
                            if (ProfileUtilities.User != null)
                            {
                                item.CREATE_BY = ProfileUtilities.User.USER_NAME;
                                item.CREATE_DATE = CurrentRepository.GetDateDatabase();
                            }
                            UnitOfWork.Repository<FormObjectRepo>().Create(item);
                        }

                        //Copy language of object
                        var lstLang = UnitOfWork.Repository<LanguageRepo>().Queryable().Where(x => x.FORM_CODE == FormCopy);
                        foreach (var item in lstLang)
                        {
                            UnitOfWork.Repository<LanguageRepo>().Detach(item);
                            item.PKID = Guid.NewGuid().ToString();
                            item.FORM_CODE = ObjDetail.CODE;
                            if (ProfileUtilities.User != null)
                            {
                                item.CREATE_BY = ProfileUtilities.User.USER_NAME;
                                item.CREATE_DATE = CurrentRepository.GetDateDatabase();
                            }
                            lstNewLang.Add(item);
                            UnitOfWork.Repository<LanguageRepo>().Create(item);
                        }
                    }

                    UnitOfWork.Commit();

                    foreach (var item in lstNewLang)
                    {
                        LanguageUtilities.AddToCache(new LanguageObject()
                        {
                            Code = item.OBJECT_TYPE + "-" + item.FORM_CODE + "-" + item.FK_CODE,
                            Language = item.LANG,
                            Value = item.VALUE
                        });
                    }
                }
                else
                {
                    State = false;
                    MesseageCode = "1101";
                }
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }

        public void GetObjectById(string id)
        {
            ObjObject = UnitOfWork.Repository<FormObjectRepo>().Get(id);
        }

        public void DeleteObject(string strLstSelected)
        {
            try
            {
                var lstId = strLstSelected.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<object>();
                UnitOfWork.BeginTransaction();

                foreach (var item in lstId)
                {
                    var obj = UnitOfWork.Repository<FormObjectRepo>().Get(item);
                    var lstLang = UnitOfWork.Repository<LanguageRepo>().Queryable().Where(x => x.FK_CODE == obj.OBJECT_CODE && x.FORM_CODE == obj.FK_FORM && x.OBJECT_TYPE == obj.TYPE).ToList();
                    UnitOfWork.Repository<LanguageRepo>().Delete(lstLang);
                    UnitOfWork.Repository<FormObjectRepo>().Delete(obj);
                }
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }

        public override void Delete(string strLstSelected)
        {
            try
            {
                var lstId = strLstSelected.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<object>();
                UnitOfWork.BeginTransaction();

                var lstLang = UnitOfWork.Repository<LanguageRepo>().Queryable().Where(x => lstId.Contains(x.FORM_CODE)).ToList();
                UnitOfWork.Repository<LanguageRepo>().Delete(lstLang);
                CurrentRepository.Delete(lstId);

                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }
    }
}
