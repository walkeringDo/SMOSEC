using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SMOSEC.DTOs.InputDTO;
using SMOSEC.UI.Layout;
using Smobiler.Core.Controls;

namespace SMOSEC.UI.ConsumablesManager
{
    partial class frmConsumablesChoose: Smobiler.Core.Controls.MobileForm
    {
        #region ����
        private AutofacConfig _autofacConfig = new AutofacConfig();//����������
        public DataTable ConTable;  //�Ĳı��
        public List<string> ConList;  //�Ĳ�
        public string LocationId;  //������

        public OperationType OperationType;  //��������

        public List<WarehouseReceiptRowInputDto> WrList=new List<WarehouseReceiptRowInputDto>();  //��ⵥ����
        public List<OutboundOrderRowInputDto> OoList=new List<OutboundOrderRowInputDto>();  //���ⵥ����
        public string UserId; //�û����
        

        #endregion

        /// <summary>
        /// ��ȫѡ�����仯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Checkall_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (Checkall.Checked)
                {
                    switch (OperationType)
                    {
                        case OperationType.����:
                            foreach (var row in listViewCon.Rows)
                            {

                                frmConChooseExLayout CRow = (frmConChooseExLayout)row.Control;
                                CRow.CheckBox1.Checked = true;
                                decimal Quant;
                                if (decimal.TryParse(CRow.lblQuant.Text, out Quant) == false)
                                {
                                    throw new Exception("�Ĳı��" + CRow.LblCId.Text + "�Ŀ���ʽ����ȷ��");
                                }
                                decimal Quantity;
                                if (decimal.TryParse(CRow.numeric1.Value.ToString(), out Quantity) == false)
                                {
                                    throw new Exception("�Ĳı��" + CRow.LblCId.Text + "��������ʽ����ȷ��");
                                }
                                decimal Money;
                                if (decimal.TryParse(CRow.numeric2.Value.ToString(), out Money) == false)
                                {
                                    throw new Exception("�Ĳı��" + CRow.LblCId.Text + "�Ľ���ʽ����ȷ��");
                                }
                                if (Quantity > Quant)
                                {
                                    throw new Exception("�Ĳı��" + CRow.LblCId.Text + "��治�㡣");
                                }
                                AddCon(CRow.LblCId.Text, Quant, Quantity, Money, CRow.Image.ResourceID, CRow.lblName.Text);
                            }
                            break;
                        case OperationType.���:
                            foreach (var row in listViewCon.Rows)
                            {
                                frmConChooseLayout CRows = (frmConChooseLayout)row.Control;
                                CRows.CheckBox1.Checked = true;
                                decimal Quantity;
                                if (decimal.TryParse(CRows.numeric1.Value.ToString(), out Quantity) == false)
                                {
                                    throw new Exception("�Ĳı��" + CRows.LblCId.Text + "��������ʽ����ȷ��");
                                }
                                decimal Money;
                                if (decimal.TryParse(CRows.numeric2.Value.ToString(), out Money) == false)
                                {
                                    throw new Exception("�Ĳı��" + CRows.LblCId.Text + "�Ľ���ʽ����ȷ��");
                                }
                                AddCon(CRows.LblCId.Text, 0, Quantity, Money, CRows.Image.ResourceID, CRows.lblName.Text);
                            }
                            break;
                    }
                }
                else
                {
                    switch (OperationType)
                    {
                        case OperationType.����:
                            foreach (var row in listViewCon.Rows)
                            {

                                frmConChooseExLayout CRow = (frmConChooseExLayout)row.Control;
                                CRow.CheckBox1.Checked = false;                                
                            }
                            break;
                        case OperationType.���:
                            foreach (var row in listViewCon.Rows)
                            {
                                frmConChooseLayout CRows = (frmConChooseLayout)row.Control;
                                CRows.CheckBox1.Checked = false;
                                
                            }
                            break;
                    }
                    ConTable.Rows.Clear();
                    ConList.Clear();
                    
                    ;

                }

            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }

            
        }

        /// <summary>
        /// ������������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Press(object sender, EventArgs e)
        {
            try
            {
                //����ǳ��⣬�жϿ�湻����
                GetCon();
                ShowResult = ShowResult.Yes;
                Close();
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }

        /// <summary>
        /// �����Ĳ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void plSearch_Press(object sender, EventArgs e)
        {
            try
            {
                GetRows();
                Bind(txtFactor.Text);
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }

        /// <summary>
        /// �����˼�����رմ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmConsumablesChoose_KeyDown(object sender, KeyDownEventArgs e)
        {
            if (e.KeyCode == KeyCode.Back)
                Close();
        }

        /// <summary>
        /// ��ʼ������ʱ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmConsumablesChoose_Load(object sender, EventArgs e)
        {
            try
            {
                //����Ӧ�Ĳ���ѡ��ͬ������ģ��
                switch (OperationType)
                {
                    case OperationType.���:
                        listViewCon.TemplateControlName = "frmConChooseLayout";
                        break;
                    case OperationType.����:
                        listViewCon.TemplateControlName = "frmConChooseExLayout";
                        break;
                }
                Bind(null);
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }


        private void Bind(string name)
        {
            try
            {
                if (ConTable == null)
                {
                    ConTable=new DataTable();
                }
                if (ConList == null)
                {
                    ConList=new List<string>();
                }
                if (ConTable.Columns.Count == 0)
                {
                    ConTable.Columns.Add("IMAGE");
                    ConTable.Columns.Add("CID");
                    ConTable.Columns.Add("NAME");
                    ConTable.Columns.Add("TYPE");
                    ConTable.Columns.Add("QUANT");
                    ConTable.Columns.Add("QUANTITY");
                    ConTable.Columns.Add("MONEY");
                    //                    AssTable.Columns.Add("IsChecked", Type.GetType("System.Boolean"));
                    //                    AssTable.Columns.Add("IsChecked");
                }
                DataColumn[] keys = new DataColumn[1];
                keys[0] = ConTable.Columns["CID"];
                ConTable.PrimaryKey = keys;

                DataTable conTable = new DataTable();
                switch (OperationType)
                {
                    case OperationType.���:
                        conTable = _autofacConfig.ConsumablesService.GetConListByName(name);
                        foreach (DataRow row in conTable.Rows)
                        {
                            if (ConList.Contains(row["CID"].ToString()))
                            {
                                WarehouseReceiptRowInputDto dto = WrList.Find(a => a.CID == row["CID"].ToString());
                                row["QUANTITY"] = dto.QUANTITY;
                                row["MONEY"] = dto.MONEY;
                                row["IsChecked"] = true;
                            }
                        }
                        break;
                    case OperationType.����:
                        conTable = _autofacConfig.ConsumablesService.GetConListByLocationAndName(LocationId, name);
                        if (conTable.Rows.Count > 0)
                        {
                            foreach (DataRow row in conTable.Rows)
                            {
                                if (ConList.Contains(row["CID"].ToString()))
                                {
                                    OutboundOrderRowInputDto dto = OoList.Find(a => a.CID == row["CID"].ToString());
                                    row["QUANTITY"] = dto.QUANTITY;
                                    row["MONEY"] = dto.MONEY;
                                    row["IsChecked"] = true;
                                }
                            }
                        }
                        
                        break;
                }
//                foreach (DataRow row in conTable.Rows)
//                {
//                    if (ConList.Contains(row["CID"].ToString()))
//                    {
//                        row["IsChecked"] = true;
//                    }
//                }

                listViewCon.Rows.Clear();
                if (conTable.Rows.Count > 0)
                {
                    listViewCon.DataSource = conTable;
                    listViewCon.DataBind();
                }
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }

        }

        public void AddCon(string CId, decimal Quant,decimal Quantity,decimal Money, string image, string name)
        {
            try
            {
                if (ConList.Contains(CId))
                {
                    DataRow row = ConTable.Rows.Find(CId);
                    row["QUANT"] = Quant;
                    row["QUANTITY"] = Quantity;
                    row["MONEY"] = Money;
//                    throw new Exception("����ӹ����ʲ���");
                }
                else
                {
                    DataRow row = ConTable.NewRow();
                    row["CID"] = CId;
                    row["IMAGE"] = image;
                    row["NAME"] = name;
                    row["QUANT"] = Quant;
                    row["QUANTITY"] = Quantity;
                    row["MONEY"] = Money;
                    //                    row["IsChecked"] = true;
                    switch (OperationType)
                    {
                        case OperationType.���:
                            row["TYPE"] = "WR";
                            break;
                        case OperationType.����:
                            row["TYPE"] = "OO";
                            break;                        
                    }
                    ConTable.Rows.Add(row);
                    ConList.Add(CId);
                }
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }

        }
        public void RemoveCon(string CId)
        {
            try
            {
                DataRow row = ConTable.Rows.Find(CId);
                ConTable.Rows.Remove(row);
                ConList.Remove(CId);
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }

        public void UpdateCheckState()
        {
            try
            {
                //                int selectcount = 0;
                if (listViewCon.Rows.Count == ConList.Count)
                {
                    Checkall.Checked = true;
                }
                else
                {
                    Checkall.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }

        private void GetRows()
        {
//            OoList.Clear();
//            WrList.Clear();
            switch (OperationType)
            {
                case OperationType.����:
                    foreach (var row in listViewCon.Rows)
                    {

                        frmConChooseExLayout CRow = (frmConChooseExLayout)row.Control;
                        if (ConList.Contains(CRow.LblCId.Text))
                        {
                            decimal Quant;
                            if (decimal.TryParse(CRow.lblQuant.Text, out Quant) == false)
                            {
                                throw new Exception("�Ĳı��" + CRow.LblCId.Text + "�Ŀ���ʽ����ȷ��");
                            }
                            decimal Quantity;
                            if (decimal.TryParse(CRow.numeric1.Value.ToString(), out Quantity) == false)
                            {
                                throw new Exception("�Ĳı��" + CRow.LblCId.Text + "��������ʽ����ȷ��");
                            }
                            decimal Money;
                            if (decimal.TryParse(CRow.numeric2.Value.ToString(), out Money) == false)
                            {
                                throw new Exception("�Ĳı��" + CRow.LblCId.Text + "�Ľ���ʽ����ȷ��");
                            }
                            if (Quantity > Quant)
                            {
                                throw new Exception("�Ĳı��" + CRow.LblCId.Text + "��治�㡣");
                            }
                            AddCon(CRow.LblCId.Text, Quant, Quantity, Money, CRow.Image.ResourceID, CRow.lblName.Text);
                            if (OoList.Any(a => a.CID == CRow.LblCId.Text))
                            {
                                OutboundOrderRowInputDto dto = OoList.Find(a => a.CID == CRow.LblCId.Text);
                                dto.MONEY = Money;
                                dto.QUANTITY = Quantity;
                            }
                            else
                            {
                                OutboundOrderRowInputDto rowInput = new OutboundOrderRowInputDto
                                {
                                    CID = CRow.LblCId.Text,
                                    MONEY = Money,
                                    NOTE = "",
                                    QUANTITY = Quantity
                                };
                                OoList.Add(rowInput);
                            }
                            
                        }
                    }
                    break;
                case OperationType.���:
                    foreach (var row in listViewCon.Rows)
                    {
                        frmConChooseLayout CRows = (frmConChooseLayout)row.Control;
                        if (ConList.Contains(CRows.LblCId.Text))
                        {
                            decimal Quantity;
                            if (decimal.TryParse(CRows.numeric1.Value.ToString(), out Quantity) == false)
                            {
                                throw new Exception("�Ĳı��" + CRows.LblCId.Text + "��������ʽ����ȷ��");
                            }
                            decimal Money;
                            if (decimal.TryParse(CRows.numeric2.Value.ToString(), out Money) == false)
                            {
                                throw new Exception("�Ĳı��" + CRows.LblCId.Text + "�Ľ���ʽ����ȷ��");
                            }
                            AddCon(CRows.LblCId.Text, 0, Quantity, Money, CRows.Image.ResourceID, CRows.lblName.Text);
                            if (WrList.Any(a => a.CID == CRows.LblCId.Text))
                            {
                                WarehouseReceiptRowInputDto dto = WrList.Find(a => a.CID == CRows.LblCId.Text);
                                dto.MONEY = Money;
                                dto.QUANTITY = Quantity;
                            }
                            else
                            {
                                WarehouseReceiptRowInputDto rowInput = new WarehouseReceiptRowInputDto
                                {
                                    CID = CRows.LblCId.Text,
                                    MONEY = Money,
                                    NOTE = "",
                                    QUANTITY = Quantity
                                };
                                WrList.Add(rowInput);
                            }
                        }
                    }
                    break;
            }


//            foreach (var row in listViewCon.Rows)
//            {
//
//                OperCreateConExLayout CRow = (OperCreateConExLayout)row.Control;
//                decimal Quant;
//                if (decimal.TryParse(CRow.lblQuant.Text, out Quant) == false)
//                {
//                    throw new Exception("�Ĳı��" + CRow.lblCId.Text + "�Ŀ���ʽ����ȷ��");
//                }
//                decimal Quantity;
//                if (decimal.TryParse(CRow.numQuant.Value.ToString(), out Quantity) == false)
//                {
//                    throw new Exception("�Ĳı��" + CRow.lblCId.Text + "��������ʽ����ȷ��");
//                }
//                decimal Money;
//                if (decimal.TryParse(CRow.numMoney.Value.ToString(), out Money) == false)
//                {
//                    throw new Exception("�Ĳı��" + CRow.lblCId.Text + "�Ľ���ʽ����ȷ��");
//                }
//                if (Quantity > Quant)
//                {
//                    throw new Exception("�Ĳı��" + CRow.lblCId.Text + "��治�㡣");
//                }
//                OutboundOrderRowInputDto rowInput = new OutboundOrderRowInputDto
//                {
//                    CID = CRow.lblCId.Text,
//                    MONEY = Money,
//                    NOTE = CRow.txtRNote.Text,
//                    QUANTITY = Quantity
//                };
//                rowsInputDtos.Add(rowInput);
//            }
        }

        private void GetCon()
        {
            switch (OperationType)
            {
                case OperationType.����:
                    ConTable.Rows.Clear();
                    ConList.Clear();
                    foreach (var row in listViewCon.Rows)
                    {

                        frmConChooseExLayout CRow = (frmConChooseExLayout) row.Control;
                        if (CRow.CheckBox1.Checked)
                        {
                            decimal Quant;
                            if (decimal.TryParse(CRow.lblQuant.Text, out Quant) == false)
                            {
                                throw new Exception("�Ĳı��" + CRow.LblCId.Text + "�Ŀ���ʽ����ȷ��");
                            }
                            decimal Quantity;
                            if (decimal.TryParse(CRow.numeric1.Value.ToString(), out Quantity) == false)
                            {
                                throw new Exception("�Ĳı��" + CRow.LblCId.Text + "��������ʽ����ȷ��");
                            }
                            decimal Money;
                            if (decimal.TryParse(CRow.numeric2.Value.ToString(), out Money) == false)
                            {
                                throw new Exception("�Ĳı��" + CRow.LblCId.Text + "�Ľ���ʽ����ȷ��");
                            }
                            if (Quantity > Quant)
                            {
                                throw new Exception("�Ĳı��" + CRow.LblCId.Text + "��治�㡣");
                            }
                            AddCon(CRow.LblCId.Text, Quant, Quantity, Money, CRow.Image.ResourceID, CRow.lblName.Text);
                        }
                    }
                    break;
                case OperationType.���:
                    ConTable.Rows.Clear();
                    ConList.Clear();
                    foreach (var row in listViewCon.Rows)
                    {
                        frmConChooseLayout CRows = (frmConChooseLayout) row.Control;
                        if (CRows.CheckBox1.Checked)
                        {
                            decimal Quantity;
                            if (decimal.TryParse(CRows.numeric1.Value.ToString(), out Quantity) == false)
                            {
                                throw new Exception("�Ĳı��" + CRows.LblCId.Text + "��������ʽ����ȷ��");
                            }
                            decimal Money;
                            if (decimal.TryParse(CRows.numeric2.Value.ToString(), out Money) == false)
                            {
                                throw new Exception("�Ĳı��" + CRows.LblCId.Text + "�Ľ���ʽ����ȷ��");
                            }
                            AddCon(CRows.LblCId.Text, 0, Quantity, Money, CRows.Image.ResourceID, CRows.lblName.Text);
                        }
                    }
                    break;
            }

            
        }
    }
}