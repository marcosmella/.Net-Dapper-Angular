import { Injectable } from "@angular/core";
import { NgxSpinnerService } from "ngx-spinner";

@Injectable({
	providedIn: "root"
})
export class LoadingService {
	constructor(public loading: NgxSpinnerService) {}

	show(name: string): void {
		setTimeout(
			() =>
				this.loading.show(name, {
					bdColor: "rgba(51,51,51,0.8)",
					size: "medium",
					color: "#fff",
					fullScreen: name === "fs" ? true : false,
					type: "ball-scale-multiple"
				}),
			25
		);
	}

	hide(name: string): void {
		this.loading.hide(name);
	}
}
