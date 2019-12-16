using System;
using System.Collections.Generic;
using Smobiler.Core.Controls;
using System.Data;
using SMOSEC.DTOs.Enum;
using System.Drawing;
using System.Windows.Forms;
using ListView = Smobiler.Core.Controls.ListView;
using SMOSEC.UI.Layout;
using SMOSEC.CommLib;
using SMOSEC.DTOs.InputDTO;
using SMOSEC.Domain.Entity;

namespace SMOSEC.UI.ConsumablesManager
{
    partial class frmConInventoryResult : Smobiler.Core.Controls.MobileForm
    {
        #region  �������
        private AutofacConfig _autofacConfig = new AutofacConfig();//����������
        public string IID; //�̵㵥���
        private string UserId;  //�û����
        private DataTable waiTable = new DataTable(); //���̵���ʲ�
        private DataTable alreadyTable = new DataTable(); //���̵���ʲ�
        private Dictionary<string, List<decimal>> conDictionary = new Dictionary<string, List<decimal>>();  //�ʲ�
        private List<string> conList;  //�ʲ��ĳ�ʼ�б�

        private ListView waitListView = new ListView();
        private ListView alreadyListView = new ListView();

        public string LocationId;
        public InventoryStatus Status;
        #endregion
        /// <summary>
        /// ҳ���ʼ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmConInventoryResult_Load(object sender, EventArgs e)
        {
            try
            {
                UserId = Client.Session["UserID"].ToString();
                //��Ӹ�������
                if (waiTable.Columns.Count == 0)
                {
                    waiTable.Columns.Add("RESULTNAME");
                    waiTable.Columns.Add("CID");
                    waiTable.Columns.Add("Image");
                    waiTable.Columns.Add("Name");
                    waiTable.Columns.Add("Specification");
                    waiTable.Columns.Add("Total");
                    waiTable.Columns.Add("RealAmount");
                }
                DataColumn[] keys = new DataColumn[1];
                keys[0] = waiTable.Columns["CID"];
                waiTable.PrimaryKey = keys;

                //��Ӹ�������
                if (alreadyTable.Columns.Count == 0)
                {
                    alreadyTable.Columns.Add("RESULTNAME");
                    alreadyTable.Columns.Add("CID");
                    alreadyTable.Columns.Add("Image");
                    alreadyTable.Columns.Add("Name");
                    alreadyTable.Columns.Add("Specification");
                    alreadyTable.Columns.Add("Total");
                    alreadyTable.Columns.Add("RealAmount");
                }
                DataColumn[] keys2 = new DataColumn[1];
                keys2[0] = alreadyTable.Columns["CID"];
                alreadyTable.PrimaryKey = keys2;

                //���ListView��tabpageview
                waitListView.TemplateControlName = "frmCIResultLayout";
                waitListView.ShowSplitLine = true;
                waitListView.SplitLineColor = Color.FromArgb(230, 230, 230);
                waitListView.Dock = DockStyle.Fill;
                tabPageView1.Controls.Add(waitListView);

                alreadyListView.TemplateControlName = "frmCIResultLayout";
                alreadyListView.ShowSplitLine = true;
                alreadyListView.SplitLineColor = Color.FromArgb(230, 230, 230);
                alreadyListView.Dock = DockStyle.Fill;
                tabPageView1.Controls.Add(alreadyListView);

                var inventory = _autofacConfig.ConInventoryService.GetConInventoryById(IID);
                lblName.Text = inventory.NAME;
                lblHandleMan.Text = inventory.HANDLEMANNAME;
                lblCount.Text = inventory.TOTAL.ToString();
                lblLocatin.Text = inventory.LOCATIONNAME;
                Status = (InventoryStatus)inventory
                    .STATUS;

                if (Status == InventoryStatus.�̵���� || Status == InventoryStatus.�̵�δ��ʼ)
                {
                    panelScan.Visible = false;
                }
                //�����Ҫ�̵���ʲ��б�
                conList = _autofacConfig.ConInventoryService.GetPendingInventoryList(IID);

                //�õ��̵㵥��ǰ����������
                conDictionary = _autofacConfig.ConInventoryService.GetResultDictionary(IID);

                //�õ����̵���ʲ��б�
                var waiTable1 = _autofacConfig.ConInventoryService.GetPendingInventoryTable(IID, LocationId);
                foreach (DataRow row in waiTable1.Rows)
                {
                    DataRow Row = waiTable.NewRow();
                    Row["CID"] = row["CID"].ToString();
                    Row["RESULTNAME"] = row["RESULTNAME"].ToString();
                    Row["Image"] = row["Image"].ToString();
                    Row["Name"] = row["Name"].ToString();
                    Row["Specification"] = row["Specification"].ToString();
                    Row["Total"] = row["Total"].ToString();
                    Row["RealAmount"] = row["RealAmount"].ToString();

                    waiTable.Rows.Add(Row);
                }
                if (inventory.TOTAL == 0)
                {
                    lblCount.Text = waiTable1.Rows.Count.ToString();
                }


                //�õ����̵���ʲ��б�
                var alreadyTable1 = _autofacConfig.ConInventoryService.GetConInventoryResultsByIID(IID, ResultStatus.���̵�);
                foreach (DataRow row in alreadyTable1.Rows)
                {
                    DataRow Row = alreadyTable.NewRow();
                    Row["CID"] = row["CID"].ToString();
                    Row["RESULTNAME"] = row["RESULTNAME"].ToString();
                    Row["Image"] = row["Image"].ToString();
                    Row["Name"] = row["Name"].ToString();
                    Row["Specification"] = row["Specification"].ToString();
                    Row["Total"] = row["Total"].ToString();
                    Row["RealAmount"] = row["RealAmount"].ToString();

                    alreadyTable.Rows.Add(Row);
                }

                if (Status == InventoryStatus.�̵���� || Status == InventoryStatus.�̵�δ��ʼ)
                {
                    Form.ActionButton.Enabled = false;
                }

                //������
                Bind();
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
        /// <summary>
        /// ���ݼ���
        /// </summary>
        private void Bind()
        {
            try
            {
                waitListView.Rows.Clear();
                waitListView.DataSource = waiTable;
                waitListView.DataBind();

                alreadyListView.Rows.Clear();
                alreadyListView.DataSource = alreadyTable;
                alreadyListView.DataBind();

                tabPageView1.Titles = new string[] {
                    "���̵�(" + waiTable.Rows.Count.ToString() + ")",
                    "���̵�(" + alreadyTable.Rows.Count.ToString() + ")" };

                foreach (var row in alreadyListView.Rows)
                {
                    frmCIResultLayout layout = (frmCIResultLayout)row.Control;
                    if (layout.label2.Text == "�̿�")
                        layout.label2.ForeColor = Color.Red;
                    else if (layout.label2.Text == "��ӯ")
                        layout.label2.ForeColor = Color.FromArgb(43, 140, 255);
                }
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
        /// <summary>
        /// �̵��ϴ����̵����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmConInventoryResult_ActionButtonPress(object sender, ActionButtonPressEventArgs e)
        {
            try
            {
                ReturnInfo rInfo = new ReturnInfo();
                switch (e.Index)
                {
                    case 0:
                        //�ϴ����
                        ConInventoryInputDto inputDto = new ConInventoryInputDto
                        {
                            IID = IID,
                            IsEnd = false,
                            ConDictionary = conDictionary,
                            LOCATIONID = LocationId,
                            CREATEUSER = UserId
                        };
                        inputDto.IsEnd = false;
                        rInfo = _autofacConfig.ConInventoryService.UpdateInventory(inputDto);
                        Toast(rInfo.IsSuccess ? "�ϴ�����ɹ�!" : rInfo.ErrorInfo);
                        break;
                    case 1:
                        //�̵����
                        Dictionary<string, List<decimal>> conDictionary2 = new Dictionary<string, List<decimal>>();
                        foreach (var key in conDictionary.Keys)
                        {
                            if (conDictionary[key][1] == (int)ResultStatus.���̵�)
                            {
                                List<decimal> list = new List<decimal>();
                                list.Add(0);
                                list.Add(Convert.ToDecimal((int)ResultStatus.�̿�));
                                conDictionary2.Add(key, list);
                            }
                            else
                            {
                                conDictionary2.Add(key, conDictionary[key]);
                            }
                        }

                        ConInventoryInputDto inputDto2 = new ConInventoryInputDto
                        {
                            IID = IID,
                            LOCATIONID = LocationId,
                            IsEnd = false,
                            ConDictionary = conDictionary2
                        };
                        inputDto2.IsEnd = true;
                        rInfo = _autofacConfig.ConInventoryService.UpdateInventory(inputDto2);
                        if (rInfo.IsSuccess)
                        {
                            ShowResult = ShowResult.Yes;
                            Close();
                            Toast("�̵�����ɹ�.");
                        }
                        else
                        {
                            Toast(rInfo.ErrorInfo);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
        /// <summary>
        /// �Ĳ�ɨ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barcodeScanner1_BarcodeScanned(object sender, BarcodeResultArgs e)
        {
            try
            {
                string CID = e.Value.ToUpper();
                var con = _autofacConfig.ConsumablesService.GetConsumablesByID(CID);
                if (con != null)
                {
                    ConInventoryResult result = _autofacConfig.ConInventoryService.GetResultByCID(IID, CID);
                    if (result != null)
                    {
                        if (result.RESULT.ToString() != "0") throw new Exception("�úĲ����̵����,�����ظ��̵�!");
                        frmCIResultTotalLayout frm = new frmCIResultTotalLayout();
                        DataTable conq = _autofacConfig.ConsumablesService.GetQuants(LocationId, CID);
                        frm.lblNumber.Text = conq.Rows[0]["QUANTITY"].ToString();
                        frm.CID = CID;
                        Form.ShowDialog(frm);
                    }
                    else        //��ӯ
                    {
                        frmCIResultTotalLayout frm = new frmCIResultTotalLayout();
                        DataTable conq = _autofacConfig.ConsumablesService.GetQuants(LocationId, CID);
                        frm.plNumber.Visible = false;
                        frm.Height = 120;
                        frm.CID = CID;
                        Form.ShowDialog(frm);
                    }


                }
                else
                {
                    Toast("δ�ҵ���Ӧ�ĺĲ�!");
                }
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
        /// <summary>
        /// �̵�Ĳģ�ˢ�½���
        /// </summary>
        /// <param name="CID"></param>
        /// <param name="RealNumber"></param>
        public void AddConToDictionary(string CID, Decimal RealAmount)
        {
            if (conList.Contains(CID))
            {
                //�����������Ҫ�̵�ģ�״̬��Ϊ���̵�
                conDictionary[CID][0] = RealAmount;
                //������̵��datatable���ڸ��ʲ����Ӵ��̵���ɾ���������뵽���̵�datatable
                DataRow row = waiTable.Rows.Find(CID);
                if (row != null)
                {
                    DataRow alreadyRow = alreadyTable.NewRow();
                    alreadyRow["CID"] = row["CID"].ToString();
                    alreadyRow["Image"] = row["Image"].ToString();
                    alreadyRow["Name"] = row["Name"].ToString();
                    alreadyRow["Specification"] = row["Specification"].ToString();
                    alreadyRow["Total"] = row["Total"].ToString();
                    alreadyRow["RealAmount"] = RealAmount;
                    if (Convert.ToDecimal(row["Total"]) < RealAmount)
                    {
                        alreadyRow["RESULTNAME"] = "��ӯ";
                        conDictionary[CID][1] = (int)ResultStatus.��ӯ;
                    }
                    else if (Convert.ToDecimal(alreadyRow["Total"]) > RealAmount)
                    {
                        alreadyRow["RESULTNAME"] = "�̿�";
                        conDictionary[CID][1] = (int)ResultStatus.�̿�;
                    }
                    else
                    {
                        alreadyRow["RESULTNAME"] = "����";
                        conDictionary[CID][1] = (int)ResultStatus.����;
                    }
                    alreadyTable.Rows.Add(alreadyRow);
                    waiTable.Rows.Remove(row);
                }
                else
                {
                    if (conDictionary[CID][1] != (int)ResultStatus.��ӯ)
                    {
                        DataRow Arow = alreadyTable.Rows.Find(CID);
                        if (Convert.ToDecimal(Arow["Total"]) < RealAmount)
                        {
                            conDictionary[CID][1] = (int)ResultStatus.��ӯ;
                            Arow["RealAmount"] = RealAmount;
                            Arow["RESULTNAME"] = "��ӯ";
                        }
                        else if (Convert.ToDecimal(Arow["Total"]) > RealAmount)
                        {
                            conDictionary[CID][1] = (int)ResultStatus.�̿�;
                            Arow["RealAmount"] = RealAmount;
                            Arow["RESULTNAME"] = "�̿�";
                        }
                        else
                        {
                            conDictionary[CID][1] = (int)ResultStatus.����;
                            Arow["RealAmount"] = RealAmount;
                            Arow["RESULTNAME"] = "";
                        }
                    }
                    else
                    {
                        DataRow Arow = alreadyTable.Rows.Find(CID);
                        if (Convert.ToDecimal(Arow["Total"]) < RealAmount)
                        {
                            Arow["RealAmount"] = RealAmount;
                        }
                        else if (Convert.ToDecimal(Arow["Total"]) >= RealAmount)
                        {
                            if (Convert.ToDecimal(Arow["Total"]) > RealAmount)
                            {
                                conDictionary[CID][1] = (int)ResultStatus.�̿�;
                                Arow["RESULTNAME"] = "�̿�";
                            }
                            else
                            {
                                conDictionary[CID][1] = (int)ResultStatus.����;
                                Arow["RESULTNAME"] = "";
                            }
                        }
                    }
                }
            }
            else
            {
                //��������ǲ���Ҫ�̵�ģ�״̬��Ϊ��ӯ
                if (!conDictionary.ContainsKey(CID))
                {
                    List<decimal> list = new List<decimal>();
                    list.Add(RealAmount);
                    list.Add(Convert.ToDecimal((int)ResultStatus.��ӯ));
                    conDictionary.Add(CID, list);
                }
                DataRow row = alreadyTable.Rows.Find(CID);
                if (row == null)
                {
                    var con = _autofacConfig.ConsumablesService.GetConsumablesByID(CID);

                    DataRow moreRow = alreadyTable.NewRow();
                    moreRow["CID"] = con.CID;
                    moreRow["RESULTNAME"] = "��ӯ";
                    moreRow["Image"] = con.IMAGE;
                    moreRow["Name"] = con.NAME;
                    moreRow["Specification"] = con.SPECIFICATION;
                    moreRow["Total"] = 0;
                    moreRow["RealAmount"] = RealAmount;
                    alreadyTable.Rows.Add(moreRow);
                }
                else
                {
                    row["RealAmount"] = RealAmount;
                }
            }
            Bind();
        }
        /// <summary>
        /// ��ά��ɨ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panelScan_Press(object sender, EventArgs e)
        {
            if (Status == InventoryStatus.�̵���� || Status == InventoryStatus.�̵�δ��ʼ)
            {
                Toast("�̵�δ��ʼ���Ѿ�����.");
            }
            else
            {
                barcodeScanner1.GetBarcode();
            }
        }
    }
}