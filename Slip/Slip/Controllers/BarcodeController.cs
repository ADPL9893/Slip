using Slip.Models;
using Slip.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZXing;
using ZXing.Common;

namespace Slip.Controllers
{
    public class BarcodeController : BaseController
    {
        public ActionResult R_PreRoughQRList(string StoneID, string JangadNo, int CakePcs, decimal CakeWeight)
        {

            List<QRList> _List = new List<QRList>();

            if (StoneID != "")
            {
                //--QR SAVE STARRT
                string QRPath = Server.MapPath("~/Upload/RQR/" + StoneID + ".png");

                BarcodeWriter writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new EncodingOptions
                    {
                        Width = 75, // Adjust the width of the QR code image
                        Height = 75, // Adjust the height of the QR code image
                        Margin = 0 // Adjust the margin of the QR code image
                    }
                };

                string QRText = StoneID;


                Bitmap qrCodeBitmap = writer.Write(QRText);
                qrCodeBitmap.Save(QRPath, System.Drawing.Imaging.ImageFormat.Png);
                //--QR SAVE END

                QRList Details = new QRList();
                Details.QRUrl = "..\\Upload\\RQR\\" + StoneID + ".png";
                Details.SubStoneID = StoneID;
                Details.JangadNo = JangadNo;
                Details.Pcs = CakePcs;
                Details.Weight = CakeWeight;

                _List.Add(Details);
                SessionFacade.QRGenerateList = _List;

            }
            else
            {
                SessionFacade.QRGenerateList = null;
            }
            return View();
        }
    }

}