using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smobiler.Core;
using Smobiler.Core.Controls;
using SMOSEC.Domain.Entity;
using System.Data;
using SMOSEC.DTOs.InputDTO;

namespace SMOSEC.UI.AssetsManager
{
    partial class frmRepairDetailSN : Smobiler.Core.Controls.MobileForm
    {
        #region "definition"
        AutofacConfig autofacConfig = new AutofacConfig();     //����������
        public String ROID;     //���޵����
        #endregion
        /// <summary>
        /// ά�޵�ȷ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Press(object sender, EventArgs e)
        {
            frmRepairDealSN frm = new frmRepairDealSN();
            frm.ROID = ROID;
            Show(frm, (MobileForm sender1, object args) =>
            {
                if (frm.ShowResult == ShowResult.Yes)
                    Bind();   //ˢ��������ʾ
            });
        }
        /// <summary>
        /// ҳ���ʼ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmRepairDetailSN_Load(object sender, EventArgs e)
        {
            Bind();
        }
        /// <summary>
        /// ��������
        /// </summary>
        public void Bind()
        {
            try
            {
                ROInputDto ROData = autofacConfig.assRepairOrderService.GetByID(ROID);
                coreUser User = autofacConfig.coreUserService.GetUserByID(ROData.HANDLEMAN);
                lblDealMan.Text = User.USER_NAME;
                DatePicker.Value = ROData.APPLYDATE;
                if (ROData.COST != 0)
                    lblPrice.Text = ROData.COST.ToString();
                lblContent.Text = ROData.REPAIRCONTENT;
                if (String.IsNullOrEmpty(ROData.NOTE)) lblNote.Text = ROData.NOTE;

                DataTable tableAssets = new DataTable();      //δ����SN���ʲ��б�
                tableAssets.Columns.Add("ASSID");             //�ʲ����
                tableAssets.Columns.Add("NAME");              //�ʲ�����
                tableAssets.Columns.Add("IMAGE");             //�ʲ�ͼƬ
                tableAssets.Columns.Add("SN");                //���к�
                tableAssets.Columns.Add("STATUS");            //����״̬
                foreach (AssRepairOrderRow Row in ROData.Rows)
                {
                    Assets assets = autofacConfig.orderCommonService.GetAssetsByID(Row.ASSID);
                    if (Row.STATUS == 0)
                    {
                        tableAssets.Rows.Add(Row.ASSID, assets.NAME, assets.IMAGE, Row.SN, "�ȴ�ά��");
                    }
                    else
                    {
                        tableAssets.Rows.Add(Row.ASSID, assets.NAME, assets.IMAGE, Row.SN, "ά�����");
                    }
                }
                if (tableAssets.Rows.Count > 0)
                {
                    ListAssetsSN.DataSource = tableAssets;
                    ListAssetsSN.DataBind();
                }
                if (Client.Session["Role"].ToString() == "SMOSECUser") plButton.Visible = false;
                //���ά�޵�����ɣ�������ά�޵�����ť
                if (ROData.STATUS == 1) plButton.Visible = false;
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
    }
}