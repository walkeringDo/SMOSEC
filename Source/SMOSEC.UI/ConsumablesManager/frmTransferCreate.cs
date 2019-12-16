using System;
using System.Collections.Generic;
using Smobiler.Core.Controls;
using SMOSEC.DTOs.InputDTO;
using SMOSEC.Domain.Entity;
using SMOSEC.UI.Layout;
using SMOSEC.CommLib;
using System.Data;
using SMOSEC.DTOs.Enum;

namespace SMOSEC.UI.ConsumablesManager
{
    partial class frmTransferCreate : Smobiler.Core.Controls.MobileForm
    {
        #region "definition"
        AutofacConfig autofacConfig = new AutofacConfig();     //����������
        public List<ConsumablesOrderRow> RowData = new List<ConsumablesOrderRow>();    //δ����SN����
        public TOInputDto TransferData = new TOInputDto();        //ά�޵���Ϣ
        public String CID;               //�Ĳı��
        #endregion
        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Press(object sender, EventArgs e)
        {
            try
            {
                if (btnDealInMan.Tag == null)
                    throw new Exception("�������Ա����Ϊ��");
                else
                    TransferData.MANAGER = btnDealInMan.Tag.ToString();     //�������Ա

                if (btnLocation.Tag == null)
                    throw new Exception("����������Ϊ��");
                else
                    TransferData.DESLOCATIONID = btnLocation.Tag.ToString();     //��������

                if (btnDealMan.Tag == null)
                    throw new Exception("�����˲���Ϊ��");
                else
                    TransferData.HANDLEMAN = btnDealMan.Tag.ToString();     //������

                TransferData.TRANSFERDATE = DatePicker.Value;   //ά�޻���
                TransferData.NOTE = txtNote.Text;                           //��ע
                TransferData.STATUS = 0;                                    //ά�޵�״̬
                TransferData.CREATEUSER = Client.Session["UserID"].ToString();      //�����û�
                TransferData.CREATEDATE = DateTime.Now;

                List<AssTransferOrderRow> Data = new List<AssTransferOrderRow>();
                if (ListAssets.Rows.Count == 0) throw new Exception("���������Ϊ��!");
                foreach (ListViewRow Row in ListAssets.Rows)
                {
                    frmOrderCreateLayout Layout = Row.Control as frmOrderCreateLayout;
                    ConsumablesOrderRow RowData = Layout.getData();
                    AssTransferOrderRow assRow = new AssTransferOrderRow();

                    assRow.IMAGE = RowData.IMAGE;
                    assRow.CID = RowData.CID;
                    assRow.INTRANSFERQTY = RowData.QTY;
                    assRow.LOCATIONID = RowData.LOCATIONID;
                    assRow.STATUS = RowData.STATUS;
                    assRow.CREATEDATE = DateTime.Now;
                    Data.Add(assRow);
                }
                TransferData.Rows = Data;
                ReturnInfo r = autofacConfig.assTransferOrderService.AddAssTransferOrder(TransferData,OperateType.�Ĳ�);
                if (r.IsSuccess)
                {
                    ShowResult = ShowResult.Yes;
                    Form.Close();          //�����ɹ�
                    Toast("�����������ɹ�!");
                }
                else
                {
                    throw new Exception(r.ErrorInfo);
                }
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
        /// <summary>
        /// �������Աѡ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDealInMan_Press(object sender, EventArgs e)
        {
            try
            {
                popDealInMan.Groups.Clear();       //�������
                PopListGroup poli = new PopListGroup();
                popDealInMan.Groups.Add(poli);
                poli.Title = "�������Աѡ��";
                List<coreUser> users = autofacConfig.coreUserService.GetDealInAdmin();
                foreach (coreUser Row in users)
                {
                    poli.AddListItem(Row.USER_NAME, Row.USER_ID);
                }
                if (btnDealMan.Tag != null)   //�������ѡ�������ʾѡ��Ч��
                {
                    foreach (PopListItem Item in poli.Items)
                    {
                        if (Item.Value == btnDealMan.Tag.ToString())
                            popDealInMan.SetSelections(Item);
                    }
                }
                popDealInMan.ShowDialog();
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
        /// <summary>
        /// ����ѡ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLocation_Press(object sender, EventArgs e)
        {
            try
            {
                popLocation.Groups.Clear();       //�������
                PopListGroup poli = new PopListGroup();
                popLocation.Groups.Add(poli);
                poli.Title = "��������ѡ��";
                List<AssLocation> users = autofacConfig.assLocationService.GetEnableAll();
                foreach (AssLocation Row in users)
                {
                    poli.AddListItem(Row.NAME, Row.LOCATIONID);
                }
                if (btnLocation.Tag != null)   //�������ѡ�������ʾѡ��Ч��
                {
                    foreach (PopListItem Item in poli.Items)
                    {
                        if (Item.Value == btnLocation.Tag.ToString())
                            popLocation.SetSelections(Item);
                    }
                }
                popLocation.ShowDialog();
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
        /// <summary>
        /// ������ѡ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDealMan_Press(object sender, EventArgs e)
        {
            try
            {
                popDealMan.Groups.Clear();       //�������
                PopListGroup poli = new PopListGroup();
                popDealMan.Groups.Add(poli);
                poli.Title = "������ѡ��";
                List<coreUser> users = autofacConfig.coreUserService.GetAdmin();
                foreach (coreUser Row in users)
                {
                    poli.AddListItem(Row.USER_NAME, Row.USER_ID);
                }
                if (btnDealMan.Tag != null)   //�������ѡ�������ʾѡ��Ч��
                {
                    foreach (PopListItem Item in poli.Items)
                    {
                        if (Item.Value == btnDealMan.Tag.ToString())
                            popDealMan.SetSelections(Item);
                    }
                }
                popDealMan.ShowDialog();
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
        /// <summary>
        /// ���ݼ���
        /// </summary>
        public void Bind()
        {
            try
            {
                DataTable tableAssets = new DataTable();       //δ����SN���ʲ��б�
                tableAssets.Columns.Add("CID");              //�ʲ����
                tableAssets.Columns.Add("NAME");               //�ʲ����� 
                tableAssets.Columns.Add("LOCATIONID");         //������ 
                tableAssets.Columns.Add("LOCATIONNAME");       //��������
                tableAssets.Columns.Add("IMAGE");              //ͼƬ���
                tableAssets.Columns.Add("QUANTITY");          //��������
                tableAssets.Columns.Add("SELECTQTY");          //ѡ������
                if (RowData.Count > 0)
                {
                    foreach (ConsumablesOrderRow Row in RowData)
                    {
                        ConQuant conQuant = autofacConfig.orderCommonService.GetUnUseConByCID(Row.CID,Row.LOCATIONID);
                        Consumables con = autofacConfig.orderCommonService.GetConsByID(Row.CID);
                        AssLocation Loc = autofacConfig.assLocationService.GetByID(Row.LOCATIONID);
                        tableAssets.Rows.Add(Row.CID, con.NAME, Row.LOCATIONID, Loc.NAME, con.IMAGE, conQuant.QUANTITY, Row.QTY);
                    }
                }
                ListAssets.DataSource = tableAssets;
                ListAssets.DataBind();
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
        /// <summary>
        /// ɾ����ǰѡ������
        /// </summary>
        /// <param name="ASSID"></param>
        public void ReMoveAss(String CID)
        {
            foreach (ConsumablesOrderRow Row in RowData)
            {
                if (Row.CID == CID)
                {
                    RowData.Remove(Row);
                    break;
                }
            }
            Bind();       //ˢ�µ�ǰҳ��
        }
        /// <summary>
        /// �ʲ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Press(object sender, EventArgs e)
        {
            try
            {
                if (btnLocation.Tag == null) throw new Exception("��ѡ���������!");
                List<ConsumablesOrderRow> Data = new List<ConsumablesOrderRow>();
                foreach (ListViewRow Row in ListAssets.Rows)
                {
                    frmOrderCreateLayout Layout = Row.Control as frmOrderCreateLayout;
                    Data.Add(Layout.getData());
                }

                frmTransferConsChoose frm = new frmTransferConsChoose();
                frm.RowData = Data;
                frm.LocInID = btnLocation.Tag.ToString();
                Show(frm, (MobileForm sender1, object args) =>
                {
                    if (frm.ShowResult == ShowResult.Yes)
                    {
                        //���¼�������
                        RowData = frm.RowData;
                        Bind();
                    }
                });
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
        /// <summary>
        /// ɨ����������ʲ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void betGet_Press(object sender, EventArgs e)
        {
            try
            {
                if (btnLocation.Tag == null) throw new Exception("��ѡ���������!");
                BarcodeScanner1.GetBarcode();
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
        /// <summary>
        /// ɨ�赽����ʱ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BarcodeScanner1_BarcodeScanned(object sender, BarcodeResultArgs e)
        {
            try
            {
                ConsumablesOrderRow Data = new ConsumablesOrderRow();
                if (string.IsNullOrEmpty(e.error))
                    CID = e.Value;
                else
                    throw new Exception(e.error);
                List<ConQuant> assList = autofacConfig.orderCommonService.GetUnUseCon(btnLocation.Tag.ToString(),CID);
                if (assList.Count > 1)
                {
                    popConLoc.Groups.Clear();
                    PopListGroup poli = new PopListGroup();
                    popConLoc.Groups.Add(poli);
                    foreach (ConQuant Row in assList)
                    {
                        if (Row.LOCATIONID != btnLocation.Tag.ToString())
                        {
                            AssLocation Loc = autofacConfig.assLocationService.GetByID(Row.LOCATIONID);
                            poli.AddListItem(Loc.NAME, Row.LOCATIONID);
                        }                       
                    }
                    popConLoc.ShowDialog();
                }
                else
                {
                    if(assList[0].LOCATIONID== btnLocation.Tag.ToString()) throw new Exception("���ʲ�����Ŀ������!");
                    Consumables cons = autofacConfig.orderCommonService.GetConsByID(CID);
                    Data.CID = CID;
                    Data.LOCATIONID = assList[0].LOCATIONID;
                    Data.IMAGE = cons.IMAGE;
                    Data.QTY = 0;
                    if (RowData.Count > 0)
                    {
                        RowData.Add(Data);
                    }
                    else
                    {
                        List<ConsumablesOrderRow> Datas = new List<ConsumablesOrderRow>();
                        Datas.Add(Data);
                        RowData = Datas;
                    }
                    Bind();
                }
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
        private void popDealMan_Selected(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(popDealMan.Selection.Text) == false)
            {
                btnDealMan.Text = popDealMan.Selection.Text + "   > ";
                btnDealMan.Tag = popDealMan.Selection.Value;         //�����˱��
            }
        }
        /// <summary>
        /// ѡ��������Ա
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void popDealInMan_Selected(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(popDealInMan.Selection.Text) == false)
            {
                btnDealInMan.Text = popDealInMan.Selection.Text + "   > ";
                btnDealInMan.Tag = popDealInMan.Selection.Value;         //�������Ա���
            }
        }
        /// <summary>
        /// ѡ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void popLocation_Selected(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(popLocation.Selection.Text) == false)     //ѡ����ĳ������
                {
                    if (btnLocation.Tag == null)     //���֮ǰû��ѡ������
                    {
                        btnLocation.Text = popLocation.Selection.Text + "   > ";
                        btnLocation.Tag = popLocation.Selection.Value;         //������                  
                    }
                    else           //֮ǰѡ��������
                    {
                        if (RowData.Count > 0)       //�������ѡ������
                        {
                            if (popLocation.Selection.Value != btnLocation.Tag.ToString())
                            {
                                MessageBox.Show("�����������򽫻������ѡ�ʲ����Ƿ������", "ϵͳ��ʾ", MessageBoxButtons.YesNo, (object sender1, MessageBoxHandlerArgs args) =>
                                {
                                    try
                                    {
                                        if (args.Result == ShowResult.Yes)
                                        {
                                            RowData.Clear();
                                            Bind();          //���°�����
                                            btnLocation.Text = popLocation.Selection.Text + "   > ";
                                            btnLocation.Tag = popLocation.Selection.Value;         //������
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Toast(ex.Message);
                                    }
                                });
                            }
                        }
                        else       //û��ѡ������
                        {
                            btnLocation.Text = popLocation.Selection.Text + "   > ";
                            btnLocation.Tag = popLocation.Selection.Value;         //������
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
        /// <summary>
        /// �Ĳ�ѡ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void popConLoc_Selected(object sender, EventArgs e)
        {
            try
            {
                if(String.IsNullOrEmpty(popConLoc.Selection.Text) == false)
                {
                    Consumables cons = autofacConfig.orderCommonService.GetConsByID(CID);
                    ConsumablesOrderRow Data = new ConsumablesOrderRow();
                    Data.CID = CID;
                    Data.LOCATIONID = popConLoc.Selection.Value;
                    Data.IMAGE = cons.IMAGE;
                    Data.QTY = 0;
                    if (RowData.Count > 0)
                    {
                        RowData.Add(Data);
                    }
                    else
                    {
                        List<ConsumablesOrderRow> Datas = new List<ConsumablesOrderRow>();
                        Datas.Add(Data);
                        RowData = Datas;
                    }
                    Bind();
                }            
            }
            catch(Exception ex)
            {
                Toast(ex.Message);
            }
        }
    }
}