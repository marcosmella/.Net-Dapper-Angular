import { NgZone } from "@angular/core";
import { Router } from "@angular/router";
import { platformBrowserDynamic } from "@angular/platform-browser-dynamic";
import { singleSpaAngular, getSingleSpaExtraProviders } from "single-spa-angular";

import { AppModule } from "./app/app.module";
import { singleSpaPropsSubject } from "./single-spa/single-spa-props";

const lifecycles = singleSpaAngular({
	bootstrapFunction: singleSpaProps => {
		singleSpaPropsSubject.next(singleSpaProps);
		return platformBrowserDynamic(getSingleSpaExtraProviders()).bootstrapModule(AppModule);
	},
	template: "<vlfrontend-health-root />",
	Router,
	NgZone
});

export const bootstrap = lifecycles.bootstrap;
export const mount = lifecycles.mount;
export const unmount = lifecycles.unmount;
