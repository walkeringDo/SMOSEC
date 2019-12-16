using System;
using System.Collections.Generic;
using Smobiler.Core.Controls;
using SMOSEC.DTOs.InputDTO;
using SMOSEC.Domain.Entity;
using System.Data;
using SMOSEC.UI.Layout;
using SMOSEC.CommLib;
using SMOSEC.DTOs.Enum;

namespace SMOSEC.UI.AssetsManager
{
    partial class frmRepairCreateSN : Smobiler.Core.Controls.MobileForm
    {
        #region "definition"
        AutofacConfig autofacConfig = new AutofacConfig();     //����������
        public List<AssetsOrderRow> SNRowData;   //����SN����
        public ROInputDto RepairData = new ROInputDto();         //ά�޵���Ϣ
        private String SN;       //���к�
        #endregion
        /// <summary>
        /// �������޵�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Press(object sender, EventArgs e)
        {
            try
            {
                if (btnDealMan.Tag ==null)
                    throw new Exception("�����˲���Ϊ��");
                else
                    RepairData.HANDLEMAN = btnDealMan.Tag.ToString();     //������                
                if (String.IsNullOrEmpty(txtPrice.Text) == false)
                    RepairData.COST = Convert.ToDecimal(txtPrice.Text);   //ά�޻���
                if (String.IsNullOrEmpty(txtContent.Text))
                    throw new Exception("ά�����ݲ���Ϊ��!");
                else
                    RepairData.REPAIRCONTENT = txtContent.Text;               //ά������

                RepairData.APPLYDATE = DatePicker.Value;                  //ҵ������
                RepairData.NOTE = txtNote.Text;                           //��ע
                RepairData.STATUS = 0;                                    //ά�޵�״̬
                RepairData.ISSNCONTROL = (Int32)ISSNCONTROL.����;         //�Ƿ�����SN
                RepairData.CREATEUSER = Client.Session["UserID"].ToString();      //�����û�
                RepairData.CREATEDATE = DateTime.Now;             

                List<AssRepairOrderRow> Data = new List<AssRepairOrderRow>();
                if (ListAssetsSN.Rows.Count == 0) throw new Exception("ά�������Ϊ��!");
                foreach (ListViewRow Row in ListAssetsSN.Rows)
                {
                    frmOrderCreateSNLayout Layout = Row.Control as frmOrderCreateSNLayout;
                    AssetsOrderRow RowData = Layout.getData();
                    AssRepairOrderRow assRow = new AssRepairOrderRow();

                    assRow.IMAGE = RowData.IMAGE;
                    assRow.ASSID = RowData.ASSID;
                    assRow.WAITREPAIRQTY = RowData.QTY;
                    assRow.SN = RowData.SN;
                    assRow.LOCATIONID = RowData.LOCATIONID;
                    assRow.STATUS = RowData.STATUS;
                    assRow.CREATEDATE = DateTime.Now;
                    Data.Add(assRow);
                }
                RepairData.Rows = Data;
                ReturnInfo r = autofacConfig.assRepairOrderService.AddAssRepairOrder(RepairData);
                if (r.IsSuccess)
                {
                    ShowResult = ShowResult.Yes;
                    Form.Close();          //�����ɹ�
                    Toast("����ά�޵��ɹ�!");
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
        /// �������
        /// </summary>
        public void Bind()
        {
            try
            {
                DataTable tableAssets = new DataTable();       //δ����SN���ʲ��б�
                tableAssets.Columns.Add("ASSID");              //�ʲ����
                tableAssets.Columns.Add("NAME");               //�ʲ����� 
                tableAssets.Columns.Add("IMAGE");              //ͼƬ���
                tableAssets.Columns.Add("SN");                 //���к�
                if (SNRowData.Count > 0)
                {
                    foreach (AssetsOrderRow Row in SNRowData)
                    {
                        Assets assets = autofacConfig.orderCommonService.GetAssetsByID(Row.ASSID);
                        tableAssets.Rows.Add(Row.ASSID, assets.NAME, assets.IMAGE, Row.SN);
                    }
                }
                ListAssetsSN.DataSource = tableAssets;
                ListAssetsSN.DataBind();
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
        /// <summary>
        /// ɾ����ǰ��ѡ����
        /// </summary>
        /// <param name="ASSID"></param>
        /// <param name="SN"></param>
        public void ReMoveAssSN(String ASSID, String SN)
        {
            try
            {
                foreach (AssetsOrderRow Row in SNRowData)
                {
                    if (Row.ASSID == ASSID && Row.SN == SN)
                    {
                        SNRowData.Remove(Row);
                        break;
                    }
                }
                Bind();       //ˢ�µ�ǰҳ��
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
        /// <summary>
        /// ɨ������ʲ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void betGet_Press(object sender, EventArgs e)
        {
            BarcodeScanner1.GetBarcode();
        }
        /// <summary>
        /// ������ѡ��
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
        /// ɨ�赽����ʱ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BarcodeScanner1_BarcodeScanned(object sender, BarcodeResultArgs e)
        {
            try
            {
                AssetsOrderRow Data = new AssetsOrderRow();
                if (string.IsNullOrEmpty(e.error))
                {
                    SN = e.Value;
                }
                else
                    throw new Exception(e.error);
                Assets assets = autofacConfig.orderCommonService.GetUnusedAssetsBySN(SN);
                if (assets == null) throw new Exception("���������к�Ϊ" + SN + "�������ʲ�");
                Data.ASSID = assets.ASSID;
                Data.LOCATIONID = assets.LOCATIONID;
                Data.IMAGE = assets.IMAGE;
                Data.QTY = 0;
                Data.SN = SN;
                if (SNRowData !=null)
                {
                    foreach(AssetsOrderRow Row in SNRowData)
                    {
                        if (Data.ASSID == Row.ASSID)
                            throw new Exception("���ʲ�����ӣ������ظ����!");
                    }
                    SNRowData.Add(Data);
                }
                else
                {
                    List<AssetsOrderRow> Datas = new List<AssetsOrderRow>();
                    Datas.Add(Data);
                    SNRowData = Datas;
                }
                Bind();        //���°�����
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
        /// <summary>
        /// �ְֳ���ɨ�赽����ʱ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void r2000Scanner1_BarcodeDataCaptured(object sender, Smobiler.Device.R2000BarcodeScanEventArgs e)
        {
            try
            {
                AssetsOrderRow Data = new AssetsOrderRow();
                SN = e.Data;
                Assets assets = autofacConfig.orderCommonService.GetUnusedAssetsBySN(SN);
                if (assets == null) throw new Exception("���������к�Ϊ" + SN + "�������ʲ�");
                Data.ASSID = assets.ASSID;
                Data.LOCATIONID = assets.LOCATIONID;
                Data.IMAGE = assets.IMAGE;
                Data.QTY = 0;
                Data.SN = SN;
                if (SNRowData != null)
                {
                    foreach (AssetsOrderRow Row in SNRowData)
                    {
                        if (Data.ASSID == Row.ASSID)
                            throw new Exception("���ʲ�����ӣ������ظ����!");
                    }
                    SNRowData.Add(Data);
                }
                else
                {
                    List<AssetsOrderRow> Datas = new List<AssetsOrderRow>();
                    Datas.Add(Data);
                    SNRowData = Datas;
                }
                Bind();        //���°�����
            }
            catch(Exception ex)
            {
                Toast(ex.Message);
            }
        }
        /// <summary>
        /// �ְֳ���ɨ�赽RFIDʱ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void r2000Scanner1_RFIDDataCaptured(object sender, Smobiler.Device.R2000RFIDScanEventArgs e)
        {
            try
            {
                AssetsOrderRow Data = new AssetsOrderRow();
                SN = e.Epc;
                Assets assets = autofacConfig.orderCommonService.GetUnusedAssetsBySN(SN);
                if (assets == null) throw new Exception();
                Data.ASSID = assets.ASSID;
                Data.LOCATIONID = assets.LOCATIONID;
                Data.IMAGE = assets.IMAGE;
                Data.QTY = 0;
                Data.SN = SN;
                if (SNRowData != null)
                {
                    foreach (AssetsOrderRow Row in SNRowData)
                    {
                        if (Data.ASSID == Row.ASSID)
                            throw new Exception();
                    }
                    SNRowData.Add(Data);
                }
                else
                {
                    List<AssetsOrderRow> Datas = new List<AssetsOrderRow>();
                    Datas.Add(Data);
                    SNRowData = Datas;
                }
                Bind();        //���°�����
            }
            catch (Exception ex)
            {
                if(ex.Message!= "��������Ϊ��System.Exception�����쳣��")
                {
                    Toast(ex.Message);
                }                
            }
        }
    }
}