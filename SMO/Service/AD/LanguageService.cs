using SMO.Core.Entities;
using SMO.Repository.Implement.AD;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Service.AD
{
    public class LanguageService : GenericService<T_AD_LANGUAGE, LanguageRepo>
    {
        public string LangSource { get; set; }
        public string LangDestination { get; set; }
        public LanguageService() : base()
        {

        }

        public void DongBo()
        {
            try
            {
                var lstSource = CurrentRepository.Queryable().Where(x => x.LANG == LangSource).ToList();
                var lstNewLang = new List<T_AD_LANGUAGE>();
                UnitOfWork.BeginTransaction();
                foreach (var item in lstSource)
                {
                    if (item.OBJECT_TYPE == "M")
                    {
                        if (!CurrentRepository.CheckExist(x => x.FK_CODE == item.FK_CODE && x.OBJECT_TYPE == item.OBJECT_TYPE && x.LANG == LangDestination))
                        {
                            var newLang = new T_AD_LANGUAGE()
                            {
                                PKID = Guid.NewGuid().ToString(),
                                FK_CODE = item.FK_CODE,
                                OBJECT_TYPE = item.OBJECT_TYPE,
                                LANG = LangDestination,
                                VALUE = item.VALUE
                            };
                            if (ProfileUtilities.User != null)
                            {
                                newLang.CREATE_BY = ProfileUtilities.User.USER_NAME;
                                newLang.CREATE_DATE = CurrentRepository.GetDateDatabase();
                            }
                            CurrentRepository.Create(newLang);
                            lstNewLang.Add(newLang);
                        }
                    }
                    else
                    {
                        if (!CurrentRepository.CheckExist(x => x.FK_CODE == item.FK_CODE && x.OBJECT_TYPE == item.OBJECT_TYPE && x.FORM_CODE == item.FORM_CODE && x.LANG == LangDestination))
                        {
                            var newLang = new T_AD_LANGUAGE()
                            {
                                PKID = Guid.NewGuid().ToString(),
                                FK_CODE = item.FK_CODE,
                                FORM_CODE = item.FORM_CODE,
                                OBJECT_TYPE = item.OBJECT_TYPE,
                                LANG = LangDestination,
                                VALUE = item.VALUE
                            };
                            if (ProfileUtilities.User != null)
                            {
                                newLang.CREATE_BY = ProfileUtilities.User.USER_NAME;
                                newLang.CREATE_DATE = CurrentRepository.GetDateDatabase();
                            }
                            CurrentRepository.Create(newLang);
                            lstNewLang.Add(newLang);
                        }
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
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }

        public void Update(string value, string id)
        {
            try
            {
                UnitOfWork.BeginTransaction();
                Get(id);
                ObjDetail.VALUE = value.Trim();
                if (ProfileUtilities.User != null)
                {
                    ObjDetail.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                    ObjDetail.UPDATE_DATE = CurrentRepository.GetDateDatabase();
                }
                CurrentRepository.Update(ObjDetail);
                UnitOfWork.Commit();

                LanguageUtilities.AddToCache(new LanguageObject()
                {
                    Code = ObjDetail.OBJECT_TYPE + "-" + ObjDetail.FORM_CODE + "-" + ObjDetail.FK_CODE,
                    Language = ObjDetail.LANG,
                    Value = ObjDetail.VALUE
                });
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
