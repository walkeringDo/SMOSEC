﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMOSEC.DTOs.OutputDTO
{
    /// <summary>
    /// 盘点单结果行项
    /// </summary>
    public class ConInventoryResultOutputDto
    {
        /// <summary>
        /// 耗材编号
        /// </summary>
        public string CID { get; set; }

        /// <summary>
        /// 盘点结果(0-待盘点,1-盘盈,2-盘亏,3-存在)
        /// </summary>
        public int RESULT { get; set; }

        /// <summary>
        /// 盘点结果
        /// </summary>
        public string RESULTNAME { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 盘点数量
        /// </summary>
        public Decimal Total { get; set; }
        /// <summary>
        /// 实盘数量
        /// </summary>
        public Decimal RealAmount { get; set; }

        /// <summary>
        /// 规格型号
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 图片名称
        /// </summary>
        public string Image { get; set; }
    }
}
