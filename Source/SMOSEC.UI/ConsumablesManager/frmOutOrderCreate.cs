using System;
using System.Collections.Generic;
using System.Data;
using Smobiler.Core.Controls;
using SMOSEC.CommLib;
using SMOSEC.Domain.Entity;
using SMOSEC.DTOs.InputDTO;
using SMOSEC.UI.Layout;

namespace SMOSEC.UI.ConsumablesManager
{
    partial class frmOutOrderCreate : Smobiler.Core.Controls.MobileForm
    {
        #region ����
        private AutofacConfig _autofacConfig = new AutofacConfig();//����������
        private string TypeId;
        private string HManId;
        private string LocationId;
        public DataTable ConTable;
        public List<string> ConList;

        private List<OutboundOrderRowInputDto> rowsInputDtos=new List<OutboundOrderRowInputDto>();
        private string UserId;
        #endregion

        /// <summary>
        /// �������ⵥ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Press(object sender, EventArgs e)
        {
            try
            {
                GetRows();
                int TId = int.Parse(TypeId);
                OutboundOrderInputDto outboundOrderInput = new OutboundOrderInputDto()
                {
                    BUSINESSDATE=DPickerCO.Value,
                    MODIFYUSER = UserId,
                    CREATEUSER = UserId,
                    HANDLEDATE = DateTime.Now,
                    LOCATIONID = LocationId,
                    NOTE = txtNote.Text,
                    HANDLEMAN = HManId,
                    RowInputDtos = rowsInputDtos,
                    TYPE = TId
                };
                ReturnInfo returnInfo = _autofacConfig.ConsumablesService.AddOutboundOrder(outboundOrderInput);
                if (returnInfo.IsSuccess)
                {
                    ShowResult = ShowResult.Yes;
                    Toast("��ӳɹ�");
                    Close();
                }
                else
                {
                    Toast(returnInfo.ErrorInfo);
                }
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }

        /// <summary>
        /// ѡ���������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBOMan_Press(object sender, EventArgs e)
        {
            try
            {
                PopBOMan.Groups.Clear();
                PopListGroup manGroup = new PopListGroup();
                PopBOMan.Title = "��������ѡ��";
                manGroup.AddListItem("�˻�", "1");
                manGroup.AddListItem("����", "2");
                PopBOMan.Groups.Add(manGroup);
                if (btnBOMan.Tag != null)   //�������ѡ�������ʾѡ��Ч��
                {
                    foreach (PopListItem Item in manGroup.Items)
                    {
                        if (Item.Value == btnBOMan.Tag.ToString())
                            PopBOMan.SetSelections(Item);
                    }
                }
                PopBOMan.ShowDialog();

            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }

        /// <summary>
        /// ѡ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLocation_Press(object sender, EventArgs e)
        {
            try
            {
                PopLocation.Groups.Clear();
                PopListGroup locationGroup = new PopListGroup();
                List<AssLocation> locations = _autofacConfig.assLocationService.GetEnableAll();
                foreach (var location in locations)
                {
                    PopListItem item = new PopListItem
                    {
                        Value = location.LOCATIONID,
                        Text = location.NAME
                    };
                    locationGroup.Items.Add(item);
                }
                PopLocation.Groups.Add(locationGroup);
                if (!string.IsNullOrEmpty(btnLocation.Text))
                {
                    foreach (PopListItem row in PopLocation.Groups[0].Items)
                    {
                        if (row.Text == btnLocation.Text)
                        {
                            PopLocation.SetSelections(row);
                        }
                    }
                }
                PopLocation.ShowDialog();
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }

        /// <summary>
        /// �����ˣ��رյ�ǰ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmOutOrderCreate_KeyDown(object sender, KeyDownEventArgs e)
        {
            if (e.KeyCode == KeyCode.Back)
                Close();
        }

        /// <summary>
        /// �������ⵥ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Press(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(btnLocation.Text))
                {
                    throw new Exception("����ѡ������.");
                }
                GetCon();
                frmConsumablesChoose consumablesChoose = new frmConsumablesChoose
                {
                    ConList = ConList,
                    ConTable = ConTable,
                    LocationId = LocationId,
                    OperationType = OperationType.����,
                    OoList = rowsInputDtos
                };
                Show(consumablesChoose, (MobileForm sender1, object args) =>
                {
                    if (consumablesChoose.ShowResult == ShowResult.Yes)
                    {
                        ConTable = consumablesChoose.ConTable;
                        ConList = consumablesChoose.ConList;
                        BindListView();
                    }
                });

            }
            catch (Exception ex)
            {
               Toast(ex.Message);
            }
        }

        /// <summary>
        /// ѡ�������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PopLocation_Selected(object sender, EventArgs e)
        {
            try
            {

                if (PopLocation.Selection != null)
                {

                    if (string.IsNullOrEmpty(btnLocation.Text))
                    {
                        LocationId = PopLocation.Selection.Value;
                    }
                    btnLocation.Text = PopLocation.Selection.Text;
                    AssLocation location = _autofacConfig.assLocationService.GetByID(LocationId);
                    if (location != null)
                    {
                        coreUser manager = _autofacConfig.coreUserService.GetUserByID(location.MANAGER);
                        HManId = location.MANAGER;
                        if (manager != null) txtHMan.Text = manager.USER_NAME;
                    }
                    if (LocationId != null && LocationId != PopLocation.Selection.Value)
                    {
                        LocationId = PopLocation.Selection.Value;
                        ClearInfo();
                    }
                }
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }

        /// <summary>
        /// �����ص�����
        /// </summary>
        private void ClearInfo()
        {
            ConTable.Rows.Clear();
            ConList.Clear();
            BindListView();
        }

        /// <summary>
        /// ������
        /// </summary>
        public void BindListView()
        {
            try
            {
                listViewCon.DataSource = ConTable;
                listViewCon.DataBind();
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }

        }

        /// <summary>
        /// �õ���������
        /// </summary>
        private void GetRows()
        {
            rowsInputDtos.Clear();
            foreach (var row in listViewCon.Rows)
            {

                OperCreateConExLayout CRow = (OperCreateConExLayout)row.Control;
                decimal Quant;
                if (decimal.TryParse(CRow.lblQuant.Text, out Quant) == false)
                {
                    throw new Exception("�Ĳı��" + CRow.lblCId.Text + "�Ŀ���ʽ����ȷ��");
                }
                decimal Quantity;
                if (decimal.TryParse(CRow.numQuant.Value.ToString(), out Quantity) == false)
                {
                    throw new Exception("�Ĳı��" + CRow.lblCId.Text + "��������ʽ����ȷ��");
                }
                decimal Money;
                if (decimal.TryParse(CRow.numMoney.Value.ToString(), out Money) == false)
                {
                    throw new Exception("�Ĳı��" + CRow.lblCId.Text + "�Ľ���ʽ����ȷ��");
                }
                if (Quantity > Quant)
                {
                    throw new Exception("�Ĳı��" + CRow.lblCId.Text + "��治�㡣");
                }
                OutboundOrderRowInputDto rowInput = new OutboundOrderRowInputDto
                {
                    CID = CRow.lblCId.Text,
                    MONEY = Money,
                    NOTE = CRow.txtRNote.Text,
                    QUANTITY = Quantity
                };
                rowsInputDtos.Add(rowInput);
            }
        }

        /// <summary>
        /// �õ���������
        /// </summary>
        private void GetCon()
        {
            foreach (var row in listViewCon.Rows)
            {

                OperCreateConExLayout CRow = (OperCreateConExLayout)row.Control;
                decimal Quant;
                if (decimal.TryParse(CRow.lblQuant.Text, out Quant) == false)
                {
                    throw new Exception("�Ĳı��" + CRow.lblCId.Text + "�Ŀ���ʽ����ȷ��");
                }
                decimal Quantity;
                if (decimal.TryParse(CRow.numQuant.Value.ToString(), out Quantity) == false)
                {
                    throw new Exception("�Ĳı��" + CRow.lblCId.Text + "��������ʽ����ȷ��");
                }
                decimal Money;
                if (decimal.TryParse(CRow.numMoney.Value.ToString(), out Money) == false)
                {
                    throw new Exception("�Ĳı��" + CRow.lblCId.Text + "�Ľ���ʽ����ȷ��");
                }
                if (Quantity > Quant)
                {
                    throw new Exception("�Ĳı��" + CRow.lblCId.Text + "��治�㡣");
                }
                DataRow Drow = ConTable.Rows.Find(CRow.lblCId.Text);
                Drow["QUANT"] = Quant;
                Drow["QUANTITY"] = Quantity;
                Drow["MONEY"] = Money;
            }

        }

        /// <summary>
        /// �����ʼ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmOutOrderCreate_Load(object sender, EventArgs e)
        {
            try
            {
                if (ConTable == null)
                {
                    ConTable = new DataTable();
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
                }
                DataColumn[] keys = new DataColumn[1];
                keys[0] = ConTable.Columns["CID"];
                ConTable.PrimaryKey = keys;

                UserId = Client.Session["UserID"].ToString();
                if (Client.Session["Role"].ToString() == "SMOSECAdmin")
                {
                    var user = _autofacConfig.coreUserService.GetUserByID(UserId);
                    LocationId = user.USER_LOCATIONID;
                    var location = _autofacConfig.assLocationService.GetByID(LocationId);
                    btnLocation.Text = location.NAME;
                    btnLocation.Enabled = false;
                    btnLocation1.Enabled = false;
                    txtHMan.Text = user.USER_NAME;
                }
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
        
        /// <summary>
        /// �Ƴ��Ĳ�
        /// </summary>
        /// <param name="CID"></param>
        public void RemoveCon(string CID)
        {
            try
            {
                DataRow row=ConTable.Rows.Find(CID);
                ConTable.Rows.Remove(row);
                ConList.Remove(CID);
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }

        /// <summary>
        /// ѡ�д����˺�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PopBOMan_Selected(object sender, EventArgs e)
        {
            if (PopBOMan.Selection != null)
            {
                TypeId = PopBOMan.Selection.Value;
                btnBOMan.Text = PopBOMan.Selection.Text;
            }
        }
    }
}