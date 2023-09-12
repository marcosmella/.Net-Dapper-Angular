import { Directive, ElementRef, AfterViewInit, HostListener, Renderer2 } from "@angular/core";
import { NgControl } from "@angular/forms";

import { TranslatePipe } from "../pipes/translate.pipe";

@Directive({
	// tslint:disable-next-line: directive-selector
	selector: "[field-validation]",
	providers: [TranslatePipe]
})
export class FieldValidationDirective implements AfterViewInit {
	constructor(private el: ElementRef, private control: NgControl, private translate: TranslatePipe, private renderer: Renderer2) {}

	ngAfterViewInit(): void {
		this.control.statusChanges.subscribe(() => {
			this.onEvent();
		});
	}

	@HostListener("blur", ["$event"])
	onEvent(): void {
		const spanId = `span#${this.el.nativeElement.id}Validation`;
		if (!document.querySelector(spanId)) {
			return;
		}
		const span = this.renderer.selectRootElement(spanId);

		if (this.control.valid) {
			this.renderer.setProperty(span, "style", "none");
		} else if (this.control.errors) {
			const error = this.readError();
			const msgError = this.translate.transform(`field${error}`);
			const msgSpecific = this.messageSpecificValue(error);

			this.renderer.setProperty(span, "innerHTML", `${msgError}${msgSpecific}`);
			this.renderer.setProperty(span, "style", "block");
		}
	}

	capitalizeFirstLetter(value: string): string {
		return value.charAt(0).toUpperCase() + value.slice(1);
	}

	readError(): any {
		let key = Object.keys(this.control.errors)[0];
		key = key.charAt(0).toUpperCase() + key.slice(1);
		// tslint:disable: ter-indent
		switch (key) {
			case "minlength":
				key += ` ${this.control.errors["minlength"]["requiredLength"]}`;
				break;
			default:
				return key;
		}
		return key;
	}

	messageSpecificValue(error: string): string {
		let value: string;
		error = error.toLowerCase();
		const key = this.control.errors[error];
		// tslint:disable: ter-indent
		switch (error) {
			case "min":
				value = `:  ${key.min.toString()}`;
				break;
			case "max":
				value = `: ${key.max.toString()}`;
				break;
			default:
				value = "";
				break;
		}
		return value;
	}
}
