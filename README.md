# CRM.Client


1 删除“新增补发订单”功能    
 屏蔽完成

2 导入订单不可导入重复项   如果重复，提示有重复项，无法导入
后端todo： crm/order/import"  后端接口导入的

3 删除“上架管理”功能
屏蔽完成

4 货币、购买数量、结算金额不在外面显示，
已完成

修改订单页面显示即可，导出的时候可以导出
已完成

成本正常显示：但是不可手动编辑。新增“采购方式”，包括“现金采购”和“使用备货”两种
todo：完成其他接口之后


密码 ：libin 1236456


前端对应后端接口如下：

修改一下  
PurchaseAccountViewModel、
PurchaseAccountView
AddPurchaseAccountView



PurchaseAccount  采购账户管理 列表 accountList  新增修改 accountEdit
FbmPurchase  FBM采购  列表  fbmList  新增修改  fbmEdit
StockProduct  备货产品  列表    stockManageList      新增修改  stockManageEdit
StockPurchase  备货采购  列表     stockList     新增修改  stockEdit