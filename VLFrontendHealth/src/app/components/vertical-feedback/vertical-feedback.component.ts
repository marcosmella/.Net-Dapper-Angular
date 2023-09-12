import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";

@Component({
	selector: "app-vertical-feedback",
	templateUrl: "./vertical-feedback.component.html",
	styleUrls: ["./vertical-feedback.component.scss"]
})
export class VerticalFeedbackComponent implements OnInit {
	@Input() feedbackTitle: string;
	@Input() description: string;
	@Input() buttonAction: string;
	@Output() onButtonAction = new EventEmitter();
	@Input() hiddenButton: boolean;
	@Input() type:
		| "dragDropUser"
		| "dragDropStructure"
		| "empty"
		| "noDocuments"
		| "noDocumentsStructure"
		| "comingSoon"
		| "systemError"
		| "addBank";
	public statusDictionary = {
		dragDropUser: "assets/svg/message/status/icon-fb-dragdrop-user.svg",
		dragDropStructure: "assets/svg/message/status/icon-fb-dragdrop-structure.svg",
		empty: "assets/svg/message/status/icon-fb-empty.svg",
		noDocuments: "assets/svg/message/status/icon-fb-no-search-document.svg",
		noDocumentsStructure: "assets/svg/message/status/icon-fb-search-document.svg",
		comingSoon: "assets/svg/message/status/icon-fb-coming-soon.svg",
		systemError: "assets/svg/message/system/astro-error-404.svg",
		addBank: "assets/svg/message/action/icon-fb-bank.svg",
		noFiles: "assets/svg/message/status/icon-fb-no-document.svg"
	};
	constructor() {}

	ngOnInit(): void {}

	applyAction(): void {
		this.onButtonAction.emit();
	}
}
