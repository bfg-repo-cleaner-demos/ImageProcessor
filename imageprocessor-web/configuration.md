---
permalink: imageprocessor-web/configuration/
layout: subpage
title: Configuration
heading: Configuration
subheading: Advanced configuration setting for editing the default behaviour.
sublinks:
 - "#webconfig|Web.Config"
 - "#securityconfig|Security.Config"
 - "#cacheconfig|Cache.Config"
 - "#processingconfig|Processing.Config"
---

<div id="webconfig">

#The Web.Config

By default ImageProcessor.Web will install references to its HttpModule within the applications `web.config`
file. This allows the library to intercept processing calls to locally stored image files.

{% highlight xml %}
<!-- ImageProcessor.Web intercept module references -->
<configuration>
  <system.web>
    <httpModules>
      <add name="ImageProcessorModule" type="ImageProcessor.Web.HttpModules.ImageProcessingModule, ImageProcessor.Web"/>
    </httpModules>
  </system.web>
  <system.webServer>
    <validation validateIntegratedModeConfiguration="false" />
    <modules>
      <add name="ImageProcessorModule" type="ImageProcessor.Web.HttpModules.ImageProcessingModule, ImageProcessor.Web"/>
    </modules>
  </system.webServer>
</configuration>
{% endhighlight %}

---

## Additional configuration

Further configuration options can be enabled by installing the [ImageProcessor.Web.Config](https://nuget.org/packages/ImageProcessor.Web.Config/)
Nuget package which will add the additional configuration values to your `web.config` file. 
    
Details for these configuration values can be found below.

{% highlight xml %}
<configuration>
  <!-- ImageProcessor.Web configuration group -->
  <configSections>
  <sectionGroup name="imageProcessor">
    <section name="security" requirePermission="false" type="ImageProcessor.Web.Configuration.ImageSecuritySection, ImageProcessor.Web"/>
    <section name="processing" requirePermission="false" type="ImageProcessor.Web.Configuration.ImageProcessingSection, ImageProcessor.Web"/>
    <section name="caching" requirePermission="false" type="ImageProcessor.Web.Configuration.ImageCacheSection, ImageProcessor.Web"/>
  </sectionGroup>
  </configSections>
  <imageProcessor >
    <security configSource="config\imageprocessor\security.config"/>
    <caching configSource="config\imageprocessor\cache.config"/>
    <processing configSource="config\imageprocessor\processing.config"/>
  </imageProcessor>
</configuration>
{% endhighlight %}
</div>
---
<div id="securityconfig">

### Security.Config

Contains setting pertaining to security configuration which allow the safe configuration of externally hosted files.
The more precise the url, the more restrictive the whitelist.

Sources for image files are plugin based. That means by implementing the `IImageService` interface and referencing the class in
configuration file you can add your own image sources.

{% highlight xml %}
<security>
  <services>
    <service name="LocalFileImageService" type="ImageProcessor.Web.Services.LocalFileImageService, ImageProcessor.Web"/>
    <service prefix="remote.axd" name="RemoteImageService" type="ImageProcessor.Web.Services.RemoteImageService, ImageProcessor.Web">
       <!-- The timeout for a request in milliseconds and the maximum 
            allowable download in bytes. -->
      <settings>
        <setting key="MaxBytes" value="4194304"/>
        <setting key="Timeout" value="3000"/>
        <!-- Added version 4.2.0. -->
        <setting key="Protocol" value="http"/>
      </settings>
      <!-- Sets allowable domains to process images from. -->
      <whitelist>
        <add url="http://images.mymovies.net"/>
        <add url="http://maps.googleapis.com" extensionLess="true" imageFormat=".png"/>
      </whitelist>
    </service>
    <!-- Add other service implemtations here. -->
  </services>
</security>
{% endhighlight %}

<div class ="alert" role="alert">

This configuration setup was added in version [4.1.0](https://www.nuget.org/packages/ImageProcessor.Web/4.1.0). For versions 
prior to that the config is as follows.

</div>

{% highlight xml %}
<!-- Whether to allow remote image requests, the timeout for a request in 
     milliseconds, the maximum allowable download in bytes and the prefix to use so that ImageProcessor can recognize 
     a remote request. -->
<security allowRemoteDownloads="true" timeout="300000" maxBytes="4194304" remotePrefix="/remote.axd">
  <!-- Sets allowable domains to process images from. -->
  <whiteList>
    <add url="http://images.mymovies.net"/>
    <add url="http://maps.googleapis.com" extensionLess="true" imageFormat=".png"/>
  </whiteList>
</security>
{% endhighlight %}
</div>
---
<div id="cacheconfig">

### Cache.Config

Contains setting pertaining to caching configuration: Where to store the cache, how many days to store images for.

Sources for image caches are plugin based. That means by implementing the `IImageCache` interface and referencing the class in
configuration file you can add your own image cache.

{% highlight xml %}
<!-- Set the currently assigned cache implemtaion here. -->
<caching currentCache="DiskCache">
  <caches>
    <cache name="DiskCache" type="ImageProcessor.Web.Caching.DiskCache, ImageProcessor.Web" maxDays="365">
      <!-- The virtual path to the disk cache location. -->      
      <settings>
        <setting key="VirtualCachePath" value="~/app_data/cache"/>
      </settings>
    </cache>
    <!-- Add other cache implemtations here. -->
  </caches>
</caching>
{% endhighlight %}

<div class ="alert" role="alert">

This configuration setup was added in version [4.2.0](https://www.nuget.org/packages/ImageProcessor.Web/4.2.0). For versions 
prior to that the config is as follows.

</div>

{% highlight xml %}
<cache virtualPath="~/app_data/cache" maxDays="365"/>
{% endhighlight %}
</div>
---
<div id="processingconfig">

### Processing.Config

Contains information and configuration for all processors.

Sources for image processors are plugin based. That means by implementing the `IGraphicsProcessor` and `IWebGraphicsProcessor` interfaces and referencing 
the web implementaion in the configuration file you can add your own image processor.

{% highlight xml %}
<processing preserveExifMetaData="false">
  <!-- Presets that allow you to reduce code in your markup. 
       Note the use of &#038; to escape ampersands. -->
  <presets>
    <preset name="demo" value="width=300&#038;height=150&#038;bgcolor=transparent"/>
  </presets>
  <!-- List of plugins. -->
  <plugins>
    <plugin name="Alpha" type="ImageProcessor.Web.Processors.Alpha, ImageProcessor.Web"/>
    <plugin name="AutoRotate" type="ImageProcessor.Web.Processors.AutoRotate, ImageProcessor.Web"/>
    <plugin name="BackgroundColor" type="ImageProcessor.Web.Processors.BackgroundColor, ImageProcessor.Web"/>
    <plugin name="Brightness" type="ImageProcessor.Web.Processors.Brightness, ImageProcessor.Web"/>
    <plugin name="Contrast" type="ImageProcessor.Web.Processors.Contrast, ImageProcessor.Web"/>
    <plugin name="Crop" type="ImageProcessor.Web.Processors.Crop, ImageProcessor.Web"/>
    <plugin name="DetectEdges" type="ImageProcessor.Web.Processors.DetectEdges, ImageProcessor.Web"/>
    <plugin name="EntropyCrop" type="ImageProcessor.Web.Processors.EntropyCrop, ImageProcessor.Web"/>
    <plugin name="Filter" type="ImageProcessor.Web.Processors.Filter, ImageProcessor.Web"/>
    <plugin name="Flip" type="ImageProcessor.Web.Processors.Flip, ImageProcessor.Web"/>
    <plugin name="Format" type="ImageProcessor.Web.Processors.Format, ImageProcessor.Web"/>
    <plugin name="GaussianBlur" type="ImageProcessor.Web.Processors.GaussianBlur, ImageProcessor.Web">
      <settings>
        <setting key="MaxSize" value="22"/>
        <setting key="MaxSigma" value="5.1"/>
        <setting key="MaxThreshold" value="100"/>
      </settings>
    </plugin>
    <plugin name="GaussianSharpen" type="ImageProcessor.Web.Processors.GaussianSharpen, ImageProcessor.Web">
      <settings>
        <setting key="MaxSize" value="22"/>
        <setting key="MaxSigma" value="5.1"/>
        <setting key="MaxThreshold" value="100"/>
      </settings>
    </plugin>
    <plugin name="Halftone" type="ImageProcessor.Web.Processors.Halftone, ImageProcessor.Web"/>
    <plugin name="Hue" type="ImageProcessor.Web.Processors.Hue, ImageProcessor.Web"/>
    <plugin name="Mask" type="ImageProcessor.Web.Processors.Mask, ImageProcessor.Web">
      <settings>
        <setting key="VirtualPath" value="~/images/imageprocessor/mask/"/>
      </settings>
    </plugin>
    <plugin name="Meta" type="ImageProcessor.Web.Processors.Meta, ImageProcessor.Web"/>
    <plugin name="Overlay" type="ImageProcessor.Web.Processors.Overlay, ImageProcessor.Web">
      <settings>
        <setting key="VirtualPath" value="~/images/imageprocessor/overlay/"/>
      </settings>
    </plugin>
    <plugin name="Pixelate" type="ImageProcessor.Web.Processors.Pixelate, ImageProcessor.Web"/>
    <plugin name="Quality" type="ImageProcessor.Web.Processors.Quality, ImageProcessor.Web"/>
    <plugin name="ReplaceColor" type="ImageProcessor.Web.Processors.ReplaceColor, ImageProcessor.Web"/>
    <plugin name="Resize" type="ImageProcessor.Web.Processors.Resize, ImageProcessor.Web">
      <settings>
        <setting key="MaxWidth" value="5000"/>
        <setting key="MaxHeight" value="5000"/>
      </settings>
    </plugin>
    <plugin name="Rotate" type="ImageProcessor.Web.Processors.Rotate, ImageProcessor.Web"/>
    <plugin name="RotateBounded" type="ImageProcessor.Web.Processors.RotateBounded, ImageProcessor.Web"/>
    <plugin name="RoundedCorners" type="ImageProcessor.Web.Processors.RoundedCorners, ImageProcessor.Web"/>
    <plugin name="Saturation" type="ImageProcessor.Web.Processors.Saturation, ImageProcessor.Web"/>
    <plugin name="Tint" type="ImageProcessor.Web.Processors.Tint, ImageProcessor.Web"/>
    <plugin name="Vignette" type="ImageProcessor.Web.Processors.Vignette, ImageProcessor.Web"/>
    <plugin name="Watermark" type="ImageProcessor.Web.Processors.Watermark, ImageProcessor.Web"/>
  </plugins>
</processing>
{% endhighlight %}
</div>