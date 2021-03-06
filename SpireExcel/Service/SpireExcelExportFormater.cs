﻿using CExcel.Service;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SpireExcel
{
    public class SpireExcelExportFormater : IExcelExportFormater<CellRange>
    {
        public virtual Action<CellRange, object> SetBodyCell()
        {
            return (c, o) =>
            {
                #region 设置单元格对齐方式   
                c.Style.HorizontalAlignment = HorizontalAlignType.Center;//水平居中
                c.Style.VerticalAlignment = VerticalAlignType.Center;//垂直居中
                #endregion

      
                c.AutoFitColumns();//单元格的宽度
                c.AutoFitRows();//行高
                c.Style.Borders[BordersLineType.EdgeLeft].LineStyle = LineStyleType.Thin;//边框
                c.Style.Borders[BordersLineType.EdgeRight].LineStyle = LineStyleType.Thin;
                c.Style.Borders[BordersLineType.EdgeTop].LineStyle = LineStyleType.Thin;
                c.Style.Borders[BordersLineType.EdgeBottom].LineStyle = LineStyleType.Thin; 
               
         
                //设置值
                c.Value = o?.ToString();
            };
        }

        public virtual Action<CellRange, object> SetHeaderCell()
        {
            return (c, o) =>
            {
                #region 设置单元格对齐方式   
                c.Style.HorizontalAlignment = HorizontalAlignType.Center;//水平居中
                c.Style.VerticalAlignment = VerticalAlignType.Center;//垂直居中
                #endregion

                c.AutoFitColumns();//单元格的宽度
                c.AutoFitRows();//行高
                c.Style.Borders[BordersLineType.EdgeLeft].LineStyle = LineStyleType.Thin;//边框
                c.Style.Borders[BordersLineType.EdgeRight].LineStyle = LineStyleType.Thin;
                c.Style.Borders[BordersLineType.EdgeTop].LineStyle = LineStyleType.Thin;
                c.Style.Borders[BordersLineType.EdgeBottom].LineStyle = LineStyleType.Thin;

                #region 设置单元格字体样式
                c.Style.Font.IsBold = true;//字体为粗体
                c.Style.Font.Color = Color.White;//字体颜色
                c.Style.Font.FontName = "微软雅黑";//字体
                c.Style.Font.Size = 12;//字体大小
                #endregion

                c.Style.Color = Color.Green;
              
                c.Value = o?.ToString();
            };
        }
    }
}
