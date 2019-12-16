using System;
using System.Collections.Generic;
using Smobiler.Core.Controls;
using SMOSEC.CommLib;
using SMOSEC.Domain.Entity;
using SMOSEC.DTOs.InputDTO;
using SMOSEC.UI.Layout;
using Smobiler.Core;

namespace SMOSEC.UI.MasterData
{
    /// <summary>
    /// �����ʲ�����
    /// </summary>
    partial class frmAssetsCreate : Smobiler.Core.Controls.MobileForm
    {
        #region ����
        public string UserId; //�û���
        public string LocationId; //������
        public string ManagerId; //�����˱��

        private AutofacConfig _autofacConfig = new AutofacConfig();//����������
        public string DepId;

        #endregion

        /// <summary>
        /// ����ʲ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Press(object sender, EventArgs e)
        { 
            try
            {
                if (string.IsNullOrEmpty(LocationId))
                {
                    throw new Exception("��ѡ������");
                }
                decimal price;
                if(btnType.Tag == null)
                {
                    throw new Exception("��ѡ�����!");
                }
                if (!decimal.TryParse(txtPrice.Text, out price))
                {
                    throw new Exception("��������ȷ�ĵ��ۣ�");
                }
                AssetsInputDto assetsInputDto = new AssetsInputDto
                {
                    AssId = txtAssID.Text,
                    BuyDate = DatePickerBuy.Value,
                    CreateUser = UserId,
                    CurrentUser = "",
                    DepartmentId = DepId,
                    ExpiryDate = DatePickerExpiry.Value,
                    Image = ImgPicture.ResourceID,
                    LocationId = LocationId,
                    Manager =ManagerId,
                    ModifyUser = UserId,
                    Name = txtName.Text,
                    Note = txtNote.Text,
                    Place = txtPlace.Text, 
                    Price = price,
                    Specification = txtSpe.Text,
                    TypeId = btnType.Tag.ToString(),
                    Unit = txtUnit.Text,
                    Vendor = txtVendor.Text,
                    SN = txtSN.Text
                };
                if (String.IsNullOrEmpty(txtPrice.Text) == false)
                    assetsInputDto.Price = decimal.Parse(txtPrice.Text);
                ReturnInfo returnInfo = _autofacConfig.SettingService.AddAssets(assetsInputDto);
                if (returnInfo.IsSuccess)
                {
                    ShowResult = ShowResult.Yes;
                    Close();
                    Toast("��ӳɹ�.�ʲ����Ϊ"+returnInfo.ErrorInfo);
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
        /// ѡ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnType_Press(object sender, EventArgs e)
        {
            try
            {
                string TypeId = "";
                if (btnType.Tag != null)
                {
                    TypeId = btnType.Tag.ToString();
                }
                frmAssTypeChooseLayout layout = new frmAssTypeChooseLayout { IsCreate = true, typeId = TypeId };
                ShowDialog(layout);
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
        /// �ϴ�ͼƬ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelImg_Press(object sender, EventArgs e)
        {
            CamPicture.GetPhoto();
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
                if (PopLocation.Selection == null) return;
                btnLocation.Text = PopLocation.Selection.Text;
                LocationId = PopLocation.Selection.Value;
                AssLocation location = _autofacConfig.assLocationService.GetByID(LocationId);
                coreUser manager = _autofacConfig.coreUserService.GetUserByID(location.MANAGER);
                ManagerId = location.MANAGER;
                txtManager.Text = manager.USER_NAME;
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
        /// <summary>
        /// ͼƬ��ȡ����
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
                    //  e.SaveFile(UserId + DateTime.Now.ToString("yyyyMMddHHmm") + ".png");
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
        /// ������ʱ���رյ�ǰ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmAssetsCreate_KeyDown(object sender, KeyDownEventArgs e)
        {
            if (e.KeyCode == KeyCode.Back)
                Close();
        }

        /// <summary>
        /// �����ʼ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmAssetsCreate_Load(object sender, EventArgs e)
        {
            try
            {
                DatePickerExpiry.Value = DateTime.Now.AddYears(1);
                UserId = Session["UserID"].ToString();
                if (Client.Session["Role"].ToString() == "SMOSECAdmin")
                {
                    var user = _autofacConfig.coreUserService.GetUserByID(UserId);
                    LocationId = user.USER_LOCATIONID;
                    var location = _autofacConfig.assLocationService.GetByID(LocationId);
                    btnLocation.Text = location.NAME;
                    btnLocation.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }

        /// <summary>
        /// �ֻ�ɨ���ά��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgBtnForAssId_Press(object sender, EventArgs e)
        {
            try
            {
                barcodeScanner1.GetBarcode();
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }

        /// <summary>
        /// �ֻ�ɨ�赽��ά����Ϣʱ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barcodeScanner1_BarcodeScanned(object sender, BarcodeResultArgs e)
        {
            try
            {
                string barCode = e.Value;
                txtSN.Text = barCode;
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }

        /// <summary>
        /// �ֳ�������ɨ���ά�룬ɨ�赽��ά��ʱ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void r2000Scanner1_BarcodeDataCaptured(object sender, Smobiler.Device.R2000BarcodeScanEventArgs e)
        {
            try
            {
                string barCode = e.Data;
                txtSN.Text = barCode;
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }

        /// <summary>
        /// �ֳ�������ɨ��RFID��ɨ�赽RFIDʱ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void r2000Scanner1_RFIDDataCaptured(object sender, Smobiler.Device.R2000RFIDScanEventArgs e)
        {
            try
            {
                string RFID = e.Epc;
                txtSN.Text = RFID;
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }

        /// <summary>
        /// ѡ����ʱ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDep_Press(object sender, EventArgs e)
        {
            try
            {
                popDep.Groups.Clear();
                PopListGroup depGroup = new PopListGroup { Title = "����" };
                var deplist = _autofacConfig.DepartmentService.GetAllDepartment();
                foreach (var dep in deplist)
                {
                    PopListItem item = new PopListItem
                    {
                        Value = dep.DEPARTMENTID,
                        Text = dep.NAME
                    };
                    depGroup.Items.Add(item);
                }
                popDep.Groups.Add(depGroup);
                if (!string.IsNullOrEmpty(DepId))
                {
                    foreach (PopListItem row in popDep.Groups[0].Items)
                    {
                        if (row.Value == DepId)
                        {
                            popDep.SetSelections(row);
                        }
                    }
                }
                popDep.ShowDialog();
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }

        /// <summary>
        /// ����ѡ��ʱ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void popDep_Selected(object sender, EventArgs e)
        {
            try
            {
                if (popDep.Selection != null)
                {
                    btnDep.Text = popDep.Selection.Text + "   > ";
                    DepId = popDep.Selection.Value;
                }
            }
            catch (Exception ex)
            {
                Toast(ex.Message);
            }
        }
    }
}