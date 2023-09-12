import { Component, OnInit, Input, ViewChild, ElementRef } from "@angular/core";
import { NgxSpinnerService } from "ngx-spinner";

@Component({
	selector: "loading-spinner",
	templateUrl: "./loading-spinner.component.html",
	styleUrls: ["./loading-spinner.component.scss"]
})
export class LoadingSpinnerComponent implements OnInit {
	@Input() name: string;
	@Input() fullScreen = false;
	@ViewChild("text", { static: false }) text: ElementRef;
	constructor(private spinner: NgxSpinnerService) {}

	ngOnInit(): void {}

	show(): void {
		setTimeout(() => {
			this.spinner.show(this.name);
		});
	}

	showAndFocus(): void {
		setTimeout(() => {
			this.spinner.show(this.name);
			setTimeout(() => this.text.nativeElement.scrollIntoView({ behavior: "smooth", block: "center" }), 10);
		});
	}

	hide(): void {
		setTimeout(() => {
			this.spinner.hide(this.name);
		});
	}
}
