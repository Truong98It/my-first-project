using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanCafe.Models;
using System.Web.Script.Serialization;
namespace WebBanCafe.Controllers
{
    public class CartController : Controller
    {
        // GET: CartItem 
        WebSite_Ban_CafeModel db = new WebSite_Ban_CafeModel();
        private const string CartSession = "CartSession";
        public ActionResult Index()
        {
            var Cart = Session[CartSession];
            List<CartItem> list = new List<CartItem>();
            if (Cart != null)
            {
                list = (List<CartItem>)Cart;
            }

            return View(list);
        }
        // Thêm sản phẩm sau đó load tới trang Giỏ hàng
        public ActionResult AddItem(string product_id)
        {
            Product product = db.Products.SingleOrDefault(n => n.Product_ID == product_id);
            var Cart = Session[CartSession];
            if (Cart != null)
            {
                //Gán list là cart đã tồn tại
                var list = (List<CartItem>)Cart;

                if (list.Exists(n => n.product.Product_ID == product_id))
                {
                    //Nếu sản phẩm đã tồn tại trong giỏ hàng update lại SL
                    foreach (var item in list)
                    {
                        if (item.product.Product_ID == product_id)
                        {
                            item.Quantity++;
                        }
                    }
                }
                else
                {   //Tạo một item mới rồi add vào list
                    CartItem item = new CartItem();
                    item.product = product;
                    item.Quantity = 1;
                    list.Add(item);
                }
                //Gán vào session
                Session[CartSession] = list;
            }
            else
            {
                // Khởi tạo một đối tượng CartItem
                CartItem item = new CartItem();
                item.product = product;
                item.Quantity = 1;
                List<CartItem> list = new List<CartItem>();
                list.Add(item);
                //Gán vào session
                Session[CartSession] = list;
            }
            return RedirectToAction("Index");
        }
        // Xóa một sản phẩm trong giỏ hàng
        public ActionResult DeleteItem(string product_id)
        {

            var Cart = Session[CartSession];
            var list = (List<CartItem>)Cart;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].product.Product_ID == product_id)
                {
                    list.Remove(list[i]);
                    break;
                }
            }
            Session[CartSession] = list;
            return RedirectToAction("Index");
        }
        // Thêm sản phẩm trong trang chi tiết
        public JsonResult ajaxAddItem(string cartModel)
        {

            var jsonCart = new JavaScriptSerializer().Deserialize<CartItem>(cartModel);
            Product product = db.Products.SingleOrDefault(a => a.Product_ID == jsonCart.product.Product_ID);
            var Cart = Session[CartSession];
            if (Cart != null)
            {
                var list = (List<CartItem>)Cart;
                if (list.Exists(n => n.product.Product_ID == jsonCart.product.Product_ID))
                {
                    // Nếu sản phẩm đã tồn tại trong giỏ thì update lại giỏ hàng
                    foreach (var item in list)
                    {
                        item.Quantity += jsonCart.Quantity;
                    }
                }
                else
                {
                    CartItem item = new CartItem();
                    item.product = product;
                    item.Quantity = jsonCart.Quantity;
                    list.Add(item);
                }
                //Gán vào session
                Session[CartSession] = list;
            }
            else
            {
                CartItem item = new CartItem();
                item.product = product;
                item.Quantity = jsonCart.Quantity;
                List<CartItem> list = new List<CartItem>();
                list.Add(item);
                //Gán session
                Session[CartSession] = list;
            }

            return Json(new
            {
                status = true
            });
        }
        //Thêm sản phẩm ở Menu
        public JsonResult ajaxAddItemMenu(string cartModel)
        {
            var jsonCart = new JavaScriptSerializer().Deserialize<CartItem>(cartModel);
            Product product = db.Products.SingleOrDefault(a => a.Product_ID == jsonCart.product.Product_ID);
            var Cart = Session[CartSession];
            if (Cart != null)
            {
                var list = (List<CartItem>)Cart;
                if (list.Exists(n => n.product.Product_ID == product.Product_ID))
                {
                    // Nếu sản phẩm đã tồn tại trong giỏ thì tăng số lượng lên 1
                    foreach (var item in list)
                    {
                        item.Quantity += 1;
                    }
                }
                else
                {
                    CartItem item = new CartItem();
                    item.product = product;
                    item.Quantity += 1;
                    list.Add(item);
                }
                //Gán vào session
                Session[CartSession] = list;
            }
            else
            {
                CartItem item = new CartItem();
                item.product = product;
                item.Quantity += 1;
                List<CartItem> list = new List<CartItem>();
                list.Add(item);
                //Gán session
                Session[CartSession] = list;
            }

            return Json(new
            {
                status = true
            });
        }
        // Update lại giỏ hàng
        public JsonResult ajaxUpdate(string cartModel)
        {
            var jsonCart = new JavaScriptSerializer().Deserialize<CartItem>(cartModel);
            var sessionCart = (List<CartItem>)Session[CartSession];
            foreach (var item in sessionCart)
            {
                //Update lại số lượng của sản phẩm
                if (item.product.Product_ID == jsonCart.product.Product_ID)
                {
                    item.Quantity = jsonCart.Quantity;
                }
            }
            Session[CartSession] = sessionCart;
            return Json(new
            {
                status = true
            });
        }

    }
}
