using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smobiler.Core;
using Smobiler.Core.Controls;
using SMOSEC.DTOs.Enum;
using SMOSEC.UI.Layout;
using SMOSEC.DTOs.InputDTO;
using SMOSEC.Domain.Entity;
using SMOSEC.CommLib;
using System.Data;

namespace SMOSEC.UI.ConsumablesManager
{
    partial class frmTransferDeal : Smobiler.Core.Controls.MobileForm
    {
        #region "definition"
        AutofacConfig autofacConfig = new AutofacConfig();     //����������
        public String TOID;     //���������
        public PROCESSMODE Type;   //��������
        #endregion
        /// <summary>
        /// ҳ���ʼ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmTransferDeal_Load(object sender, EventArgs e)
        {
            Bind();
        }
        /// <summary>
        /// ���ݼ���
        /// </summary>
        public void Bind()
        {
            try
            {
                if (Type == PROCESSMODE.����ȷ��) title1.TitleText = "������ȷ��";
                if (Type == PROCESSMODE.����ȡ��) title1.TitleText = "������ȡ��";
                TOInputDto TOData = autofacConfig.assTransferOrderService.GetByID(TOID);
                coreUser DeanInUser = autofacConfig.coreUserService.GetUserByID(TOData.MANAGER);
                coreUser DealUser = autofacConfig.coreUserService.GetUserByID(TOData.HANDLEMAN);
                AssLocation assLocation = autofacConfig.assLocationService.GetByID(TOData.DESLOCATIONID);
                lblTDInMan.Text = DeanInUser.USER_NAME;
                lblDealMan.Text = DealUser.USER_NAME;
                lblLocation.Text = assLocation.NAME;
                DatePicker.Value = TOData.TRANSFERDATE;
                if (String.IsNullOrEmpty(TOData.NOTE)) lblNote.Text = TOData.NOTE;

                DataTable tableAssets = new DataTable();       //δ����SN���ʲ��б�
                tableAssets.Columns.Add("TOROWID");           //���޵�������
                tableAssets.Columns.Add("LOCATIONID");         //������
                tableAssets.Columns.Add("LOCATIONNAME");       //��������
                tableAssets.Columns.Add("CID");              //�ʲ����
                tableAssets.Columns.Add("NAME");               //�ʲ�����
                tableAssets.Columns.Add("IMAGE");              //ͼƬ���
                tableAssets.Columns.Add("INTRANSFERQTY");      //����������
                foreach (AssTransferOrderRow Row in TOData.Rows)
                {
                    Consumables cons = autofacConfig.orderCommonService.GetConsByID(Row.CID);
                    AssLocation Location = autofacConfig.assLocationService.GetByID(Row.LOCATIONID);
                    if (Row.STATUS == 0)
                    {
                        tableAssets.Rows.Add(Row.TOROWID, Row.LOCATIONID, Location.NAME, Row.CID, cons.NAME, Row.IMAGE, Row.INTRANSFERQTY);
                    }
                }
                if (tableAssets.Rows.Count > 0)
                {
                    ListAssets.DataSource = tableAssets;
                    ListAssets.DataBind();
                }
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
        /// <summary>
        /// ȫѡ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Checkall_CheckedChanged(object sender, EventArgs e)
        {
            foreach (ListViewRow Row in ListAssets.Rows)
            {
                frmAssTDLayout Layout = Row.Control as frmAssTDLayout;
                Layout.setCheck(Checkall.Checked);
            }
        }
        /// <summary>
        /// ����ȫѡ��״̬
        /// </summary>
        public void upCheckState()
        {
            if (getNum() == ListAssets.Rows.Count)
                Checkall.Checked = true;          //ѡ����������ʱ
            else
                Checkall.Checked = false;        //û��ѡ����������
        }
        /// <summary>
        /// ���㵱ǰѡ������
        /// </summary>
        /// <returns></returns>
        public Int32 getNum()
        {
            Int32 selectQty = 0;        //��ǰѡ��������
            foreach (ListViewRow Row in ListAssets.Rows)
            {
                frmAssTDLayout Layout = Row.Control as frmAssTDLayout;
                selectQty += Layout.checkNum();
            }
            return selectQty;
        }
        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Press(object sender, EventArgs e)
        {
            try
            {
                if (getNum() == 0) throw new Exception("��ѡ��ȷ������!");

                TOInputDto BasicData = new TOInputDto();
                BasicData.MODIFYDATE = DateTime.Now;
                BasicData.MODIFYUSER = Client.Session["UserID"].ToString();
                BasicData.TOID = TOID;

                List<AssTransferOrderRow> Data = new List<AssTransferOrderRow>();
                foreach (ListViewRow Row in ListAssets.Rows)
                {
                    frmAssTDLayout Layout = Row.Control as frmAssTDLayout;
                    if (Layout.getData() != null)
                    {
                        Data.Add(Layout.getData());
                    }
                }
                BasicData.Rows = Data;
                ReturnInfo r = autofacConfig.assTransferOrderService.UpdateAssTransferOrder(BasicData,Type,OperateType.�Ĳ�);
                if (r.IsSuccess)
                {
                    ShowResult = ShowResult.Yes;
                    Form.Close();
                    if (Type == PROCESSMODE.����ȷ��)
                    {
                        Toast("ȷ�ϵ����ɹ�!");
                    }
                    else
                    {
                        Toast("ȡ�������ɹ�!");
                    }
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
    }
}