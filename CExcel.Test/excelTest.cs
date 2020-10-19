using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using CExcel.Attributes;
using CExcel.Exceptions;
using CExcel.Extensions;
using CExcel.Service;
using CExcel.Service.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Style;
using Microsoft.Extensions.DependencyInjection;
namespace CExcel.Test
{
    [TestClass]
    public class excelTest
    {
        private readonly IExcelExportService<ExcelPackage> exportService = null;
        private readonly IExcelImportService<ExcelPackage> excelImportService = null;
        private readonly IWorkbookBuilder<ExcelPackage> workbookBuilder;
        public excelTest()
        {
            var provider = Ioc.AddCExcelService();
            exportService = provider.GetService<IExcelExportService<ExcelPackage>>();
            excelImportService = provider.GetService<IExcelImportService<ExcelPackage>>();
            workbookBuilder = provider.GetService<IWorkbookBuilder<ExcelPackage>>();
        }
        /// <summary>
        /// ����
        /// </summary>
        [TestMethod]
        public void Export()
        {

            IList<Student> students = new List<Student>();
            for (int i = 0; i < 100; i++)
            {
                Student student = new Student()
                {
                    Id = i,
                    Name = $"����{i}",
                    Sex = 2,
                    Email = $"aaa{i}@123.com",
                    //CreateAt = DateTime.Now.AddDays(-1).AddMinutes(i),
                };
                students.Add(student);
            }
            try
            {
                var excelPackage = exportService.Export<Student>(students).AddSheet<Student>().AddSheet<Student>().AddSheet<Student>().AddSheet<Student>();


                FileInfo fileInfo = new FileInfo("a.xlsx");
                excelPackage.SaveAs(fileInfo);
            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// ����
        /// </summary>
        [TestMethod]
        public void Import()
        {
            ExcelPackage ep = null;

            try
            {
                using (var fs = File.Open("a.xlsx", FileMode.Open))
                    ep = workbookBuilder.CreateWorkbook(fs);

                var result = excelImportService.Import<Student>(ep);

            }
            catch (ExportExcelException ex)
            {
                ep.AddErrors<Student>(ex.ExportExcelErrors);
                FileInfo fileInfo = new FileInfo("b.xlsx");
                ep.SaveAs(fileInfo);
            }
            catch (Exception ex) { }

        }

        /// <summary>
        /// �������
        /// </summary>
        [TestMethod]
        public void AddError()
        {
            try
            { 
                var fs = File.Open("a.xlsx", FileMode.Open);
                var ep = workbookBuilder.CreateWorkbook(fs);
                fs.Close();
                IList<ExportExcelError> errors = new List<ExportExcelError>();
                ExportExcelError a = new ExportExcelError(2, 3, "�����");
                ExportExcelError b = new ExportExcelError(3, 3, "�����11133");
                errors.Add(a);
                errors.Add(b);

                ep.AddErrors<Student>(errors);
                var fs1 = File.Open("a.xlsx", FileMode.Open, FileAccess.ReadWrite);
                ep.SaveAs(fs1);
            }
            catch (Exception ex)
            {
            }

        }


    }


    [Excel("ѧ����Ϣ", true, typeof(StudentExcelTypeFormater))]
    public class Student
    {
        /// <summary>
        /// ����
        /// </summary>
        [ExcelColumn("Id", 1)]
        public int Id { get; set; }

        [ExcelColumn("����", 2)]
        [EmailAddress(ErrorMessage = "���������ʽ")]
        public string Name { get; set; }


        [ExcelColumn("�Ա�", 3, typeof(SexExcelTypeFormater), typeof(SexExcelImportFormater))]
        public int Sex { get; set; }


        [ExcelColumn("����", 4)]
        [EmailAddress]
        public string Email { get; set; }

        //[ExportColumn("����ʱ��", 4, typeof(CreateAtExcelTypeFormater), typeof(CreateAtExcelImportFormater))]
        //public DateTime CreateAt { get; set; }
    }
 

 


    public class StudentExcelTypeFormater : DefaultExcelTypeFormater
    {
        public override Action<ExcelWorksheet> SetExcelWorksheet()
        {
            return (s) =>
            {
                var address = typeof(Student).GetPropertyAddress(nameof(Student.Email));
                address = $"{address}2:{address}1000";
                var val2 = s.DataValidations.AddCustomValidation(address);
                val2.ShowErrorMessage = true;
                val2.ShowInputMessage = true;
                val2.PromptTitle = "�Զ��������ϢPromptTitle";
                val2.Prompt = "�Զ������Prompt";
                val2.ErrorTitle = "����������ErrorTitle";
                val2.Error = "����������Error";
                val2.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                var formula = val2.Formula;
                formula.ExcelFormula = $"=COUNTIF({address},\"?*@*.*\")";
            };

        }

    }


    public class SexExcelTypeFormater : DefaultExcelExportFormater
    {
        public override Action<ExcelRangeBase, object> SetBodyCell()
        {
            return (c, o) =>
            {
                if (int.TryParse(o.ToString(), out int intValue))
                {
                    if (intValue == 1)
                    {
                        c.Value = "��";
                    }
                    else if (intValue == 2)
                    {
                        c.Value = "Ů";
                    }
                    else
                    {
                        c.Value = "δ֪";
                    }

                }
                else
                {
                    c.Value = "δ֪";
                }

            };
        }


    }

    public class SexExcelImportFormater : IExcelImportFormater
    {
        public object Transformation(object origin)
        {
            if (origin == null)
            {
                return 0;
            }
            else if (origin?.ToString() == "��")
            {
                return 1;
            }
            else if (origin?.ToString() == "Ů")
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }
    }
}
