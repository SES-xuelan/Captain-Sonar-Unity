package plugin.albert.wechatsdk;

import com.tencent.mm.sdk.modelmsg.SendMessageToWX;
import com.tencent.mm.sdk.modelmsg.WXMediaMessage;
import com.tencent.mm.sdk.modelmsg.WXTextObject;
import com.unity3d.player.UnityPlayer;

import android.util.Log;
import android.widget.Toast;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;

import android.content.Context;
import android.provider.Settings;
import android.util.StringBuilderPrinter;

import com.tencent.mm.sdk.modelmsg.SendMessageToWX;
import com.tencent.mm.sdk.modelmsg.WXImageObject;
import com.tencent.mm.sdk.modelmsg.WXMediaMessage;
import com.tencent.mm.sdk.modelmsg.WXTextObject;
import com.tencent.mm.sdk.modelmsg.WXVideoObject;
import com.tencent.mm.sdk.modelmsg.WXWebpageObject;
import com.tencent.mm.sdk.openapi.IWXAPI;
import com.tencent.mm.sdk.openapi.WXAPIFactory;

public class WeChatUnityPlugin {

    private static String TAG = "Unity:WeChatUnityPlugin";

    private static final int THUMB_SIZE = 150;
    private static WeChatUnityPlugin instance;

    public static final int WECHAT_SHARE_WAY_TEXT = 1;   //文字
    public static final int WECHAT_SHARE_WAY_PICTURE = 2; //图片
    public static final int WECHAT_SHARE_WAY_WEBPAGE = 3;  //链接
    public static final int WECHAT_SHARE_WAY_VIDEO = 4; //视频
    public static final int WECHAT_SHARE_TYPE_TALK = SendMessageToWX.Req.WXSceneSession;  //会话
    public static final int WECHAT_SHARE_TYPE_FRENDS = SendMessageToWX.Req.WXSceneTimeline; //朋友圈
    public static final int TIMELINE_SUPPORTED_VERSION = 0x21020001;


    private static IWXAPI api;

    public WeChatUnityPlugin() {
        instance = new WeChatUnityPlugin();
    }

    //要使你的程序启动后微信终端能响应你的程序，必须在代码中向微信终端注册你的app_id
    //app_id 是从微信开放平台官方上申请到的，合法的appId
    public static void RegistToWeChat(String app_id) {
        Context context = UnityPlayer.currentActivity.getApplicationContext();
        api = WXAPIFactory.createWXAPI(context, app_id, true);
        api.registerApp(app_id);
    }

    public static boolean CheckWeChatInstalled() {
        return api.isWXAppInstalled() && api.isWXAppSupportAPI();
    }

    public static boolean GetWeChatTimelineSupport() {
        return api.getWXAppSupportAPI() >= TIMELINE_SUPPORTED_VERSION;
    }

    //isTalk是否发送到会话，true 发送到会话；false发送到朋友圈
    public static void ShareText(String text, boolean isTalk) {
        WXTextObject textObj = new WXTextObject();
        textObj.text = text;

        WXMediaMessage msg = new WXMediaMessage();
        msg.mediaObject = textObj;
        msg.description = text;

        //构造req
        SendMessageToWX.Req req = new SendMessageToWX.Req();
        req.transaction = buildTransaction("text");//transaction字段用于唯一标识一个请求
        if (!isTalk && GetWeChatTimelineSupport()) {
            req.scene = WECHAT_SHARE_TYPE_FRENDS;
        } else {
            req.scene = WECHAT_SHARE_TYPE_TALK;
        }
        api.sendReq(req);
    }

    public static void SharePicture(boolean isTalk) {
        Context context = UnityPlayer.currentActivity.getApplicationContext();
        Bitmap bitmap = BitmapFactory.decodeResource(context.getResources(), R.drawable.send_img);
        WXImageObject imgObj = new WXImageObject(bitmap);

        WXMediaMessage msg = new WXMediaMessage();
        msg.mediaObject = imgObj;

        Bitmap thumbBitmap = Bitmap.createScaledBitmap(bitmap, THUMB_SIZE, THUMB_SIZE, true);
        bitmap.recycle();
        msg.thumbData = Util.bmpToByteArray(thumbBitmap, true);  //设置缩略图

        SendMessageToWX.Req req = new SendMessageToWX.Req();
        req.transaction = buildTransaction("picture");
        req.message = msg;
        if (!isTalk && GetWeChatTimelineSupport()) {
            req.scene = WECHAT_SHARE_TYPE_FRENDS;
        } else {
            req.scene = WECHAT_SHARE_TYPE_TALK;
        }
        api.sendReq(req);
    }

    public static void ShareLink(String url, String title, String description, boolean isTalk) {
        Context context = UnityPlayer.currentActivity.getApplicationContext();
        WXWebpageObject webpage = new WXWebpageObject();
        webpage.webpageUrl = url;
        WXMediaMessage msg = new WXMediaMessage(webpage);
        msg.title = title;
        msg.description = description;

        Bitmap thumb = BitmapFactory.decodeResource(context.getResources(), R.drawable.send_link_thumb);
        if (thumb == null) {
            Toast.makeText(context, "图片不能为空", Toast.LENGTH_SHORT).show();
        } else {
            msg.thumbData = Util.bmpToByteArray(thumb, true);
        }

        SendMessageToWX.Req req = new SendMessageToWX.Req();
        req.transaction = buildTransaction("link");
        req.message = msg;
        if (!isTalk && GetWeChatTimelineSupport()) {
            req.scene = WECHAT_SHARE_TYPE_FRENDS;
        } else {
            req.scene = WECHAT_SHARE_TYPE_TALK;
        }
        api.sendReq(req);
    }

    public static void ShareVideo(String videoUrl, String title, String description, boolean isTalk) {
        Context context = UnityPlayer.currentActivity.getApplicationContext();
        WXVideoObject video = new WXVideoObject();
        video.videoUrl = videoUrl;

        WXMediaMessage msg = new WXMediaMessage(video);
        msg.title = title;
        msg.description = description;
        Bitmap thumb = BitmapFactory.decodeResource(context.getResources(), R.drawable.send_music_thumb);
//      BitmapFactory.decodeStream(new URL(video.videoUrl).openStream());
        /**
         * 测试过程中会出现这种情况，会有个别手机会出现调不起微信客户端的情况。造成这种情况的原因是微信对缩略图的大小、title、description等参数的大小做了限制，所以有可能是大小超过了默认的范围。
         * 一般情况下缩略图超出比较常见。Title、description都是文本，一般不会超过。
         */
        Bitmap thumbBitmap = Bitmap.createScaledBitmap(thumb, THUMB_SIZE, THUMB_SIZE, true);
        thumb.recycle();
        msg.thumbData = Util.bmpToByteArray(thumbBitmap, true);

        SendMessageToWX.Req req = new SendMessageToWX.Req();
        req.transaction = buildTransaction("video");
        req.message = msg;
        if (!isTalk && GetWeChatTimelineSupport()) {
            req.scene = WECHAT_SHARE_TYPE_FRENDS;
        } else {
            req.scene = WECHAT_SHARE_TYPE_TALK;
        }
        api.sendReq(req);
    }


    private static String buildTransaction(final String type) {
        return (type == null) ? String.valueOf(System.currentTimeMillis()) : type + System.currentTimeMillis();
    }
}
