using System;
using SMOSEC.CommLib;
using SMOSEC.DTOs.InputDTO;
using Smobiler.Core.Controls;
using Smobiler.Core;
namespace SMOSEC.UI.ConsumablesManager
{
    partial class frmConsumablesCreate : Smobiler.Core.Controls.MobileForm
    {
        #region ����
        private AutofacConfig _autofacConfig = new AutofacConfig();//����������

        public string UserId;  //�û����
        

        #endregion

        /// <summary>
        /// ����Ĳ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Press(object sender, EventArgs e)
        {
            try
            {
                //�ж���Ч��
                int? Ceiling=null;
                if (!string.IsNullOrEmpty(txtCeiling.Text))
                {
                    int result;
                    if (int.TryParse(txtCeiling.Text, out result))
                    {
                        Ceiling = result;
                    }
                    else
                    {
                        throw new Exception("��������ȷ�İ�ȫ�������.");
                    }
                }
                int? Floor = null;
                if (!string.IsNullOrEmpty(txtFloor.Text))
                {
                    int result;
                    if (int.TryParse(txtFloor.Text, out result))
                    {
                        Floor = result;
                    }
                    else
                    {
                        throw new Exception("��������ȷ�İ�ȫ�������.");
                    }
                }
                int? SPQ = null;
                if (!string.IsNullOrEmpty(txtSPQ.Text))
                {
                    int result;
                    if (int.TryParse(txtSPQ.Text, out result))
                    {
                        SPQ = result;
                    }
                    else
                    {
                        throw new Exception("��������ȷ�ı�׼��װ����.");
                    }
                }
                ConsumablesInputDto consumablesInputDto = new ConsumablesInputDto()
                {
                    CREATEUSER = UserId,
                    IMAGE = ImgPicture.ResourceID,
                    MODIFYUSER = UserId,
                    NAME = txtName.Text,
                    NOTE = txtNote.Text,
                    SAFECEILING = Ceiling,
                    SAFEFLOOR = Floor,
                    SPECIFICATION = txtSpe.Text,
                    SPQ = SPQ,
                    UNIT =txtUnit.Text
                };
                ReturnInfo returnInfo = _autofacConfig.ConsumablesService.AddConsumables(consumablesInputDto);
                if (returnInfo.IsSuccess)
                {
                    ShowResult = ShowResult.Yes;
                    Close();
                    Toast("��ӳɹ�.");
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
        /// �ϴ���Ƭ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelImg_Press(object sender, EventArgs e)
        {
            CamPicture.GetPhoto();
        }

        /// <summary>
        /// �����ˣ���رմ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmConsumablesCreate_KeyDown(object sender, KeyDownEventArgs e)
        {
            if (e.KeyCode == KeyCode.Back)
                Close();
        }

        /// <summary>
        /// ��ȡ�����ϴ���ͼƬʱ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CamPicture_ImageCaptured(object sender, BinaryResultArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(e.error))
                {
                    e.SaveFile(UserId + DateTime.Now.ToString("yyyyMMddHHmm") + ".png",MobileResourceManager.DefaultImagePath);
                    // e.SaveFile(UserId + DateTime.Now.ToString("yyyyMMddHHmm") + ".png");
                    ImgPicture.ResourceID = UserId + DateTime.Now.ToString("yyyyMMddHHmm");
                    ImgPicture.Refresh();
                }
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }

        /// <summary>
        /// �����ʼ��ʱ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmConsumablesCreate_Load(object sender, EventArgs e)
        {
            try
            {
                UserId = Session["UserID"].ToString();
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
    }
}