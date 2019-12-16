using System;
using System.Data;
using Smobiler.Core.Controls;

namespace SMOSEC.UI.ConsumablesManager
{
    partial class frmOutboundOrder : Smobiler.Core.Controls.MobileForm
    {
        #region ����
        private AutofacConfig _autofacConfig = new AutofacConfig();//����������
        

        #endregion 

        /// <summary>
        /// ��ӳ��ⵥ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Press(object sender, EventArgs e)
        {
            frmOutOrderCreate outOrderCreate = new frmOutOrderCreate();
            Show(outOrderCreate, (MobileForm sender1, object args) =>
            {
                if (outOrderCreate.ShowResult == ShowResult.Yes)
                {
                    Bind();
                }
            });
        }

        /// <summary>
        /// �����ˣ���رյ�ǰ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmOutboundOrder_KeyDown(object sender, KeyDownEventArgs e)
        {
            if (e.KeyCode == KeyCode.Back)
                Client.Exit();
        }

        /// <summary>
        /// ��ʼ������ʱ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmOutboundOrder_Load(object sender, EventArgs e)
        {
            Bind();
        }

        /// <summary>
        /// ������
        /// </summary>
        private void Bind()
        {
            try
            {
                string LocationId = "";
                string UserId = Session["UserID"].ToString();
                if (Client.Session["Role"].ToString() == "SMOSECAdmin")
                {
                    var user = _autofacConfig.coreUserService.GetUserByID(UserId);
                    LocationId = user.USER_LOCATIONID;
                }
                if (Client.Session["Role"].ToString() == "SMOSECUser")
                {
                    panel1.Visible = false;
                }
                DataTable ooList = _autofacConfig.ConsumablesService.GetOOListByUserId(Client.Session["Role"].ToString() == "SMOSECUser" ? Client.Session["UserID"].ToString() : "",LocationId);
                if (ooList != null && ooList.Rows.Count > 0)
                {
                    gridAssRows.DataSource = ooList;
                    gridAssRows.DataBind();
                }
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }

        }


    }
}