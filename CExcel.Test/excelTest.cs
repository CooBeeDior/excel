using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using CExcel.Attributes;
using CExcel.Extensions;
using CExcel.Service;
using CExcel.Service.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Style;

namespace CExcel.Test
{
    [TestClass]
    public class excelTest
    {
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
                var exportService = new ExcelExportService();

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
            try
            {
                var excelImportService = new ExcelImportService();
                var fs = File.Open("a.xlsx", FileMode.Open);
                var ep = ExcelExcelPackageBuilder.CreateExcelPackage(fs);

                var result = excelImportService.Import<Student>(ep, "ѧ����Ϣ1");
                fs.Close();
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
        [ExportColumn("Id", 1)]
        public int Id { get; set; }

        [ExportColumn("����", 2)]
        public string Name { get; set; }


        [ExportColumn("�Ա�", 3, typeof(SexExcelTypeFormater), typeof(SexExcelImportFormater))]
        public int Sex { get; set; }


        [ExportColumn("����", 4)]
        public string Email { get; set; }

        //[ExportColumn("����ʱ��", 4, typeof(CreateAtExcelTypeFormater), typeof(CreateAtExcelImportFormater))]
        //public DateTime CreateAt { get; set; }
    }
    public class CreateAtExcelImportFormater : DefaultExcelImportFormater
    {
        public override object Transformation(object origin)
        {

            var date = DateTime.ParseExact(origin.ToString(), "yyyy��MM��dd�� HH:mm:ss", null);
            return date;
        }
    }

    public class CreateAtExcelTypeFormater : DefaultExcelExportFormater
    {
        public override Action<ExcelRangeBase, object> SetBodyCell()
        {
            return (c, o) =>
            {
                c.Style.Fill.PatternType = ExcelFillStyle.Solid;
                c.Style.Fill.BackgroundColor.SetColor(Color.Green);
                c.Style.Numberformat.Format = "yyyy��MM��dd�� HH:mm:ss"; 
                c.Style.ShrinkToFit = false;//��Ԫ���Զ���Ӧ��С
                //c.AddComment(o.ToString(), $"ʱ��:{o.ToString("yyyy/MM/dd HH:mm:ss")}");
           
                //c.Worksheet.Column(typeof(Student).GetPropertyIndex(nameof(Student.CreateAt))).Width = 50;
                c.Value = o;
            };
        }

        public override Action<ExcelRangeBase, object> SetHeaderCell()
        {

            return (c, o) =>
            {
                base.SetHeaderCell()(c, o);
                c.Style.Font.Color.SetColor(Color.Black);//������ɫ
                c.Style.Fill.PatternType = ExcelFillStyle.Solid;
                c.Style.Fill.BackgroundColor.SetColor(Color.Red);
                c.AddComment(o?.ToString() ?? "", "��������Ա1");

            };
        }
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
