import { Component, Input } from "@angular/core";

@Component({
	selector: "app-button",
	templateUrl: "./button.component.html",
	styleUrls: ["./button.component.scss"]
})
export class ButtonComponent {
	@Input() set type(type: string) {
		this.buttonType = type;
	}

	@Input() set class(value: string) {
		this.buttonClass = value;
		this.buttonClassOriginal = value;
	}

	@Input()
	set disabled(isDisabled: boolean) {
		this._isDisabled = isDisabled;
		if (isDisabled) {
			this.buttonClass = this.buttonClass.replace(/primary|secondary/g, "disabled");
		} else {
			this.buttonClass = this.buttonClassOriginal;
		}
	}
	get isDisabled(): boolean {
		return this._isDisabled;
	}

	buttonType: string;
	buttonClassOriginal: string;
	buttonClass: string;
	private _isDisabled: boolean;

	constructor() {}
}
