import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";

@Component({
	selector: "app-horizontal-feedback",
	templateUrl: "./horizontal-feedback.component.html",
	styleUrls: ["./horizontal-feedback.component.scss"]
})
export class HorizontalFeedbackComponent implements OnInit {
	@Input() feedbackTitle: string;
	@Input() description: string;
	@Input() buttonAction: string;
	@Output() onButtonAction = new EventEmitter();
	@Input() hiddenButton: boolean;
	@Input() idButton = "feedbackAction";
	@Input() type:
		| "addAsset"
		| "addDocument"
		| "addEducation"
		| "addEmail"
		| "addEmergency"
		| "addGenerate"
		| "addImport"
		| "addPassword"
		| "addPhone"
		| "addRelatives"
		| "systemError"
		| "addBank"
		| "addJob"
		| "addVaccine"
		| "addDiagnostic";
	public statusDictionary = {
		addAsset: "assets/svg/message/action/icon-fb-assets.svg",
		addDocument: "assets/svg/message/action/icon-fb-document.svg",
		addEducation: "assets/svg/message/action/icon-fb-education.svg",
		addEmail: "assets/svg/message/action/icon-fb-email.svg",
		addEmergency: "assets/svg/message/action/icon-fb-emergency.svg",
		addGenerate: "assets/svg/message/action/icon-fb-generate.svg",
		addImport: "assets/svg/message/action/icon-fb-import.svg",
		addPassword: "assets/svg/message/action/icon-fb-password.svg",
		addPhone: "assets/svg/message/action/icon-fb-phone.svg",
		addRelatives: "assets/svg/message/action/icon-fb-relatives.svg",
		systemError: "assets/svg/message/system/astro-error-404.svg",
		addBank: "assets/svg/message/action/icon-fb-bank.svg",
		addJob: "assets/svg/message/action/icon-fb-job.svg",
		addVaccine: "assets/svg/message/action/icon-fb-vaccine.svg",
		addDiagnostic: "assets/svg/message/action/icon-fb-diagnostic.svg"
	};
	constructor() {}

	ngOnInit(): void {}

	applyAction(): void {
		this.onButtonAction.emit();
	}
}
