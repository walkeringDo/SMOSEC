using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smobiler.Core;
using Smobiler.Core.Controls;
using SMOSEC.Domain.Entity;
using SMOSEC.DTOs.InputDTO;
using SMOSEC.CommLib;

namespace SMOSEC.UI.ConsumablesManager
{
    partial class frmConInventoryEdit : Smobiler.Core.Controls.MobileForm
    {
        #region  �������
        private AutofacConfig _autofacConfig = new AutofacConfig();//����������

        public string IID;
        #endregion
        /// <summary>
        /// ҳ���ʼ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmConInventoryEdit_Load(object sender, EventArgs e)
        {
            try
            {
                String UserId = Client.Session["UserID"].ToString();
                var assInventory = _autofacConfig.ConInventoryService.GetConInventoryById(IID);
                txtName.Text = assInventory.NAME;
                btnManager.Text = assInventory.HANDLEMANNAME + "   > ";
                btnManager.Tag = assInventory.HANDLEMAN;
                btnLocation.Text = assInventory.LOCATIONNAME + "   > ";
                btnLocation.Tag = assInventory.LOCATIONID;
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
                List<AssLocation> users = _autofacConfig.assLocationService.GetAll();
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
        /// �̵���ѡ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnManager_Press(object sender, EventArgs e)
        {
            try
            {
                popMan.Groups.Clear();
                PopListGroup manGroup = new PopListGroup { Title = "�̵���ѡ��" };
                List<coreUser> users = _autofacConfig.coreUserService.GetAll();
                foreach (coreUser Row in users)
                {
                    manGroup.AddListItem(Row.USER_NAME, Row.USER_ID);
                }
                popMan.Groups.Add(manGroup);
                if (btnManager.Tag != null)   //�������ѡ�������ʾѡ��Ч��
                {
                    foreach (PopListItem Item in manGroup.Items)
                    {
                        if (Item.Value == btnManager.Tag.ToString())
                            popMan.SetSelections(Item);
                    }
                }
                popMan.ShowDialog();
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
        /// <summary>
        /// ѡ���̵���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void popMan_Selected(object sender, EventArgs e)
        {
            if (popMan.Selection != null)
            {
                btnManager.Text = popMan.Selection.Text + "   > ";
                btnManager.Tag = popMan.Selection.Value;
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
                if (popLocation.Selection != null)
                {
                    if(popLocation.Selection.Value != btnLocation.Tag.ToString())
                    {
                        AssLocation loc = _autofacConfig.assLocationService.GetByID(popLocation.Selection.Value);
                        var assInventory = _autofacConfig.ConInventoryService.GetConInventoryById(IID);
                        if(loc.LOCATIONID != assInventory.LOCATIONID)
                        {
                            if (loc.ISLOCKED == 1) throw new Exception("������������, ������ѡ��!");
                        }                   
                        if (loc.ISENABLE == 0) throw new Exception("�������ѽ���, ������ѡ��!");
                        btnLocation.Text = popLocation.Selection.Text + "   > ";
                        btnLocation.Tag = popLocation.Selection.Value;
                    }                   
                }
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
        /// <summary>
        /// �����̵��޸�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Press(object sender, EventArgs e)
        {
            try
            {
                AddCIResultInputDto conInventoryInput = new AddCIResultInputDto()
                {
                    IID = IID,
                    HANDLEMAN = btnManager.Tag.ToString(),
                    LocationId = btnLocation.Tag.ToString(),
                    UserId = Client.Session["UserID"].ToString(),
                    Name = txtName.Text
                };
                ReturnInfo returnInfo = _autofacConfig.ConInventoryService.UpdateInventoryOnly(conInventoryInput);
                if (returnInfo.IsSuccess)
                {
                    ShowResult = ShowResult.Yes;
                    Toast("�޸ĳɹ�");
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
    }
}