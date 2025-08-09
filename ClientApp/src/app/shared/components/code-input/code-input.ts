import { Component, ElementRef, ViewChildren, QueryList, AfterViewInit, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';


@Component({
  selector: 'app-code-input',
  imports: [CommonModule, FormsModule],
  templateUrl: './code-input.html',
  styleUrl: './code-input.scss'
})
export class CodeInput implements AfterViewInit {
  @Output() fullCode = new EventEmitter();
  @ViewChildren('codeInput') codeInputs!: QueryList<ElementRef<HTMLInputElement>>;
  codeBoxes = Array(6).fill(0);
  code: string[] = Array(6).fill('');
  currentFocusIndex = 0;

  ngAfterViewInit() {
    this.focusInput(0);
  }

  onCodeInput(event: any, index: number) {
    const input = event.target;
    const value = input.value;

    this.code[index] = value;

    if (value && index < this.codeBoxes.length - 1) {
      this.currentFocusIndex = index + 1;
      this.focusInput(this.currentFocusIndex);
    }

     this.fullCodeOutput();
  }

  onCodeKeydown(event: KeyboardEvent, index: number) {
    if (event.key === 'Backspace' && !this.code[index] && index > 0) {
      this.currentFocusIndex = index - 1;
      this.focusInput(this.currentFocusIndex);
      event.preventDefault();
    }

    if (event.key === 'ArrowLeft' && index > 0) {
      this.currentFocusIndex = index - 1;
      this.focusInput(this.currentFocusIndex);
      event.preventDefault();
    }
    if (event.key === 'ArrowRight' && index < this.codeBoxes.length - 1) {
      this.currentFocusIndex = index + 1;
      this.focusInput(this.currentFocusIndex);
      event.preventDefault();
    }

    if (event.key === 'Home') {
      this.currentFocusIndex = 0;
      this.focusInput(0);
      event.preventDefault();
    }
    if (event.key === 'End') {
      this.currentFocusIndex = this.codeBoxes.length - 1;
      this.focusInput(this.codeBoxes.length - 1);
      event.preventDefault();
    }
  }

  onCodePaste(event: ClipboardEvent) {
    event.preventDefault();
    const clipboardData = event.clipboardData?.getData('text/plain').trim();

    if (clipboardData) {
      const chars = clipboardData.split('').slice(0, this.codeBoxes.length);

      chars.forEach((char, i) => {
        this.code[i] = char;
      });

      this.codeInputs.forEach((input, i) => {
        input.nativeElement.value = this.code[i] || '';
      });

      const lastFilledIndex = chars.length - 1;
      this.currentFocusIndex = Math.min(lastFilledIndex + 1, this.codeBoxes.length - 1);
      this.focusInput(this.currentFocusIndex);
      this.fullCodeOutput();
    }
  }

  clearCode() {
    this.code = Array(6).fill('');
    this.codeInputs.forEach(input => input.nativeElement.value = '');
    this.focusInput(0);
    this.fullCodeOutput();
  }

  // #region Private Methods
  private focusInput(index: number) {
    if (this.codeInputs && this.codeInputs.toArray()[index]) {
      const input = this.codeInputs.toArray()[index].nativeElement;
      input.focus();
      input.select();
    }
  }

  private fullCodeOutput() {
    this.fullCode.emit(this.code.join(''));
  }
  // #endregion
}
